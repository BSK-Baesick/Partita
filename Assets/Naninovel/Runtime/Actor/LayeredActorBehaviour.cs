// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// When applied to a <see cref="GameObject"/>, containing child objects with <see cref="Renderer"/> components (layers), 
    /// handles the composition (layers enabled state) and rendering to a texture in back to front order based on z-position and sort order.
    /// Will prevent the child renderers from being rendered by the cameras at play mode.
    /// </summary>
    public abstract class LayeredActorBehaviour : MonoBehaviour
    {
        [Serializable]
        public class CompositionMapItem
        {
            public string Key = default;
            [TextArea(1, 5)]
            public string Composition = default;
        }

        /// <summary>
        /// Current layer composition of the actor.
        /// </summary>
        public virtual string Composition => string.Join(splitLiteral, layers.Select(l => $"{l.Group}{(l.Enabled ? enableLiteral : disableLiteral)}{l.Name}"));
        public virtual bool Animated => animated;
        public IReadOnlyDictionary<string, string> CompositionMap => compositionMap.ToDictionary(i => i.Key, i => i.Composition);

        protected virtual bool Reversed => reversed;
        protected virtual Material SharedRenderMaterial => renderMaterial;

        private const string selectLiteral = ">";
        private const string enableLiteral = "+";
        private const string disableLiteral = "-";
        private const string splitLiteral = ",";
        private static readonly string[] splitLiterals = { splitLiteral };

        [Tooltip("Whether the actor should be rendered every frame. Enable when animating the layers or implementing other dynamic behaviour.")]
        [SerializeField] private bool animated = false;
        [Tooltip("Whether to render the layers in a reversed order.")]
        [SerializeField] private bool reversed = false;
        [Tooltip("Shared material to use when rendering the layers. Will use layer renderer's material when not assigned.")]
        [SerializeField] private Material renderMaterial = default;
        [Tooltip("Allows to map layer composition expressions to keys; the keys can then be used to specify layered actor appearances instead of the full expressions.")]
        [SerializeField] private List<CompositionMapItem> compositionMap = new List<CompositionMapItem>();

        private readonly List<LayeredActorLayer> layers = new List<LayeredActorLayer>();
        private Vector2 canvasSize;
        private RenderCanvas renderCanvas;

        /// <summary>
        /// Applies provided layer composition expression to the actor.
        /// </summary>
        /// <remarks>
        /// Value format: path/to/object/parent>SelectedObjName,another/path+EnabledObjName,another/path-DisabledObjName,...
        /// Select operator (>) means that only this object is enabled inside the group, others should be disabled.
        /// When no target objects provided, all the layers inside the group will be affected (recursively, including child groups).
        /// </remarks>
        public virtual void ApplyComposition (string value)
        {
            if (layers is null || layers.Count == 0 || string.IsNullOrEmpty(value)) return;

            value = value.Trim();

            if (compositionMap.Any(i => i.Key == value))
                value = compositionMap.First(i => i.Key == value).Composition;

            var expressions = value.Split(splitLiterals, StringSplitOptions.RemoveEmptyEntries);

            foreach (var expression in expressions)
            {
                if (ParseExpression(expression, selectLiteral, out var group, out var name)) // Select expression (>).
                {
                    if (string.IsNullOrEmpty(name)) // Enable all in the group, disable all in neighbour groups.
                    {
                        var parentGroup = group.Contains("/") ? group.GetBeforeLast("/") : string.Empty;
                        ForEachLayer(l => l.Group.StartsWithFast(parentGroup), l => l.Enabled = l.Group.EqualsFast(group), group);
                    }
                    else ForEachLayer(l => l.Group.EqualsFast(group), l => l.Enabled = l.Name.EqualsFast(name), group);
                }
                else if (ParseExpression(expression, enableLiteral, out group, out name)) // Enable expression (+).
                {
                    if (string.IsNullOrEmpty(name))
                        ForEachLayer(l => l.Group.StartsWithFast(group), l => l.Enabled = true, group);
                    else ForLayer(group, name, l => l.Enabled = true);
                }
                else if (ParseExpression(expression, disableLiteral, out group, out name)) // Disable expression (-).
                {
                    if (string.IsNullOrEmpty(name))
                        ForEachLayer(l => l.Group.StartsWithFast(group), l => l.Enabled = false, group);
                    else ForLayer(group, name, l => l.Enabled = false);
                }
                else
                {
                    Debug.LogWarning($"Unrecognized `{gameObject.name}` layered actor composition expression: `{expression}`.");
                    continue;
                }
            }

            bool ParseExpression (string expression, string operationLiteral, out string group, out string name)
            {
                group = expression.GetBefore(operationLiteral);
                if (group is null) { name = null; return false; }
                name = expression.Substring(group.Length + operationLiteral.Length);
                return true;
            }

            void ForEachLayer (Func<LayeredActorLayer, bool> selector, Action<LayeredActorLayer> action, string group)
            {
                var layers = this.layers.Where(selector).ToArray();
                if (!layers.Any()) Debug.LogWarning($"`{gameObject.name}` layered actor composition group `{group}` not found.");
                else foreach (var layer in layers)
                        action.Invoke(layer);
            }

            void ForLayer (string group, string name, Action<LayeredActorLayer> action)
            {
                var layer = layers.FirstOrDefault(l => l.Group.EqualsFast(group) && l.Name.EqualsFast(name));
                if (layer is null) Debug.LogWarning($"`{gameObject.name}` layered actor layer `{name}` inside composition group `{group}` not found.");
                else action.Invoke(layer);
            }
        }

        /// <summary>
        /// Renders the enabled layers scaled by <paramref name="pixelsPerUnit"/> to the provided or a temporary <see cref="RenderTexture"/>.
        /// Don't forget to release unused render textures.
        /// </summary>
        /// <param name="pixelsPerUnit">PPU to use when rendering.</param>
        /// <param name="renderTexture">Render texture to render the content into; when not provided, will create a temporary one.</param>
        /// <returns>Temporary render texture created when no render texture is provided.</returns>
        public virtual RenderTexture Render (int pixelsPerUnit, RenderTexture renderTexture = null)
        {
            if (layers is null || layers.Count == 0)
            {
                Debug.LogWarning($"Can't render layered actor `{name}`: layers data is empty. Make sure the actor prefab contains child objects with at least one renderer.");
                return null;
            }

            var renderDimensions = canvasSize * pixelsPerUnit;
            var renderTextureSize = new Vector2Int(Mathf.RoundToInt(renderDimensions.x), Mathf.RoundToInt(renderDimensions.y));

            if (!ObjectUtils.IsValid(renderTexture))
                renderTexture = RenderTexture.GetTemporary(renderTextureSize.x, renderTextureSize.y, 24, RenderTextureFormat.ARGB32);

            Graphics.SetRenderTarget(renderTexture);
            GL.Clear(true, true, Color.clear);
            GL.PushMatrix();
 
            var orthoMin = Vector3.Scale(-renderDimensions / 2f, transform.parent.localScale) + transform.position * pixelsPerUnit;
            var orthoMax = Vector3.Scale(renderDimensions / 2f, transform.parent.localScale) + transform.position * pixelsPerUnit;
            var orthoMatrix = Matrix4x4.Ortho(orthoMin.x, orthoMax.x, orthoMin.y, orthoMax.y, float.MinValue, float.MaxValue);
            var rotationMatrix = Matrix4x4.Rotate(Quaternion.Inverse(transform.parent.localRotation));
            GL.LoadProjectionMatrix(orthoMatrix);

            if (Reversed)
                for (int i = layers.Count - 1; i >= 0; i--)
                    RenderLayerAt(i);
            else for (int i = 0; i < layers.Count; i++)
                    RenderLayerAt(i);

            void RenderLayerAt (int layerIndex)
            {
                var layer = layers[layerIndex];
                if (!layer.Enabled) return;
                var renderMaterial = SharedRenderMaterial ? SharedRenderMaterial : layer.RenderMaterial;
                renderMaterial.mainTexture = layer.Texture;
                renderMaterial.color = layer.Color;
                var renderPosition = transform.TransformPoint(rotationMatrix // Compensate actor (parent game object) rotation.
                    .MultiplyPoint3x4(transform.InverseTransformPoint(layer.Position)));
                var renderTransform = Matrix4x4.TRS(renderPosition * pixelsPerUnit, layer.Rotation, layer.Scale * pixelsPerUnit);
                for (int i = 0; i < renderMaterial.passCount; i++)
                    if (renderMaterial.SetPass(i))
                        Graphics.DrawMeshNow(layer.Mesh, renderTransform);
            }

            GL.PopMatrix();

            return renderTexture;
        }
        
        protected virtual void Awake ()
        {
            BuildLayerData();
        }

        protected virtual void OnDrawGizmos ()
        {
            if (!renderCanvas) // Render canvas draws its own gizmo.
                Gizmos.DrawWireCube(transform.position, canvasSize);
        }

        protected virtual void OnDestroy ()
        {
            foreach (var layer in layers)
                if (layer.RenderMaterial != null)
                    ObjectUtils.DestroyOrImmediate(layer.RenderMaterial);
        }

        protected virtual void BuildLayerData ()
        {
            layers.Clear();
            
            var renderers = GetComponentsInChildren<Renderer>()
                .OrderBy(s => s.sortingOrder)
                .ThenByDescending(s => s.transform.position.z).ToList();

            if (renderers.Count == 0) return;

            if (TryGetComponent<RenderCanvas>(out renderCanvas))
                canvasSize = renderCanvas.Size;
            else
            {
                var maxPosX = renderers.Max(r => Mathf.Max(Mathf.Abs(r.bounds.max.x), Mathf.Abs(r.bounds.min.x)));
                var maxPosY = renderers.Max(r => Mathf.Max(Mathf.Abs(r.bounds.max.y), Mathf.Abs(r.bounds.min.y)));
                canvasSize = new Vector2(maxPosX * 2, maxPosY * 2);
            }

            foreach (var renderer in renderers)
            {
                if (renderer is SpriteRenderer spriteRenderer)
                {
                    if (!spriteRenderer.sprite) continue;
                    layers.Add(new LayeredActorLayer(spriteRenderer));
                    continue;
                }
                
                if (!renderer.TryGetComponent<MeshFilter>(out var meshFilter)) continue;
                layers.Add(new LayeredActorLayer(renderer, meshFilter.sharedMesh ? meshFilter.sharedMesh : meshFilter.mesh));
            }
        }
    }
}
