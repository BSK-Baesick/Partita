using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using Naninovel;

public interface IFmodAudioManager : IEngineService<FmodAudioConfiguration>
{

    void StartMusicDuet(int parameterNumber);

    void StartMusicFinale(int milliaParameterint, int stephanParameterint, int veraParameterint, int paschaParameterint, int zurabParameterint);

}
