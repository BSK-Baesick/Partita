#ifndef SASAMI_NOISE_INCLUDED
#define SASAMI_NOISE_INCLUDED

#include "UnityCG.cginc"


// TRANSFORM_TEX相当の処理(vertexシェーダーでの変換)
fixed2 TRANSFORM_NOISE_TEX(fixed2 uv, fixed4 tilingOffset, fixed4 sizeScroll) {
    // 仮想ノイズテクスチャサイズ, Tiling, Offsetの適用
    uv = uv * tilingOffset.xy * sizeScroll.xy + tilingOffset.zw;

    // Scrollの適用。Tilingに合わせた相対速度でUVスクロールを行う
    uv += fixed2(sizeScroll.z * tilingOffset.x, -sizeScroll.w * tilingOffset.y) * _Time.y;

    return uv;
}


// シェーダー疑似乱数(fragmentシェーダーでUVを入力し0.0～1.0を返す/texture size周期でループする処理を追加)
fixed rand (fixed2 uv, fixed2 size) {
    uv = frac(uv/size);
    return frac(sin(dot(frac(uv/size), fixed2(12.9898,78.233))) * 43758.5453) * 0.99999;
}

// ランダムな2次元ベクトル(parlin勾配ベクトル/0～1範囲で生成した乱数を-1～1の範囲(*2-1)に拡張)
fixed2 randVec2(fixed2 uv, fixed2 size){
    return normalize(fixed2( rand(uv, size), rand(uv+fixed2(127.1,311.7), size) ) * 2.0 + -1.0);
}

// バイリニア補間(4点を与えてその矩形内のxy両方向に線形補間)
fixed2 bilinear(fixed f0, fixed f1, fixed f2, fixed f3, fixed fx, fixed fy){
    return lerp( lerp(f0, f1, fx), lerp(f2, f3, fx), fy );
}

// fade関数(ease曲線をつくる。easing関数)
fixed fade(fixed t) { return t * t * t * (t * (t * 6.0 - 15.0) + 10.0); }

// ブロックノイズ(モザイク化)
fixed blockNoise(fixed2 uv, fixed2 size)
{
    return rand(floor(uv), size);
}

// 単位バリューノイズ(ブロックノイズをバイリニア補完したノイズ)
fixed valueNoiseOne(fixed2 uv, fixed2 size)
{
    fixed2 p = floor(uv);
    fixed2 f = frac(uv);

    float f00 = rand( p+fixed2(0,0), size );
    float f10 = rand( p+fixed2(1,0), size );
    float f01 = rand( p+fixed2(0,1), size );
    float f11 = rand( p+fixed2(1,1), size );
    return bilinear( f00, f10, f01, f11, fade(f.x), fade(f.y) );
}

// バリューノイズ(オクターブの異なるノイズテクスチャを5枚合成)
fixed valueNoise(fixed2 uv, fixed2 size)
{
    fixed f = 0;
    f += valueNoiseOne( uv* 2, size )/2;
    f += valueNoiseOne( uv* 4, size )/4;
    f += valueNoiseOne( uv* 8, size )/8;
    f += valueNoiseOne( uv*16, size )/16;
    f += valueNoiseOne( uv*32, size )/32;
    return f;
}

// 単位パーリンノイズ(ランダムに決定した格子点の勾配ベクトルと、格子点から内部点へのベクトルとの内積をバイリニア補完)
fixed perlinNoiseOne(fixed2 uv, fixed2 size) 
{
    fixed2 p = floor(uv);
    fixed2 f = frac(uv);

    fixed d00 = dot( randVec2(p+fixed2(0,0), size), f-fixed2(0,0) );
    fixed d10 = dot( randVec2(p+fixed2(1,0), size), f-fixed2(1,0) );
    fixed d01 = dot( randVec2(p+fixed2(0,1), size), f-fixed2(0,1) );
    fixed d11 = dot( randVec2(p+fixed2(1,1), size), f-fixed2(1,1) );
    return bilinear( d00, d10, d01, d11, fade(f.x), fade(f.y) ) + 0.5f;
}

// パーリンノイズ(オクターブの異なるノイズテクスチャを5枚合成)
fixed perlinNoise(fixed2 uv, fixed2 size) 
{
    fixed f = 0;
    f += perlinNoiseOne( uv* 2, size )/2;
    f += perlinNoiseOne( uv* 4, size )/4;
    f += perlinNoiseOne( uv* 8, size )/8;
    f += perlinNoiseOne( uv*16, size )/16;
    f += perlinNoiseOne( uv*32, size )/32;
    return f;
}


// 法線マップ用ノイズ(x:0.0 ～ 1.0, x:0.0 ～ 1.0, z:1.0)
fixed3 normalNoise( fixed2 uv, fixed2 size )
{
    fixed3 result = fixed3( perlinNoise(uv.xy, size),
                            perlinNoise(uv.xy+fixed2(1,1), size),
                            1.0 );
    return result;
}


#endif
