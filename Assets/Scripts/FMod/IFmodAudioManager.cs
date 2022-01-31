using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using Naninovel;

public interface IFmodAudioManager : IEngineService<FmodAudioConfiguration>
{

    void PlayMusicDuet(int parameterNumber);

    void PlayMusicFinale(int milliaParameterint, int stephanParameterint, int veraParameterint, int paschaParameterint, int zurabParameterint);

}
