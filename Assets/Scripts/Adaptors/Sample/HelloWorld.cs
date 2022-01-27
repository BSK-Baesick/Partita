using Naninovel;
using Naninovel.Commands;
using UniRx.Async;
using UnityEngine;

/// <summary>
/// Will print the "Hello World" string in the console using the @PrintHelloWorld command at Inky
/// </summary>

[CommandAlias("PrintHelloWorld")]
public class HelloWorld : Command
{

    public override async UniTask ExecuteAsync(CancellationToken asyncToken = default)
    {
        Debug.Log("Hello World");
    }
}

