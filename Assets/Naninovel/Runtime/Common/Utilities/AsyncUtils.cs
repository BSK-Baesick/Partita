﻿// Copyright 2017-2020 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UniRx.Async;

namespace Naninovel
{
    public static class AsyncUtils
    {
        public static YieldAwaitable WaitEndOfFrame => UniTask.Yield(PlayerLoopTiming.PostLateUpdate);

        public static UniTask.Awaiter GetAwaiter (this UniTask? task) => task?.GetAwaiter() ?? UniTask.CompletedTask.GetAwaiter();

        public static UniTask<T>.Awaiter GetAwaiter<T> (this UniTask<T>? task) => task?.GetAwaiter() ?? UniTask.FromResult<T>(default).GetAwaiter();

        #if !UNITASK2_AVAILABLE // For compatibility with UniTask v2 (it doesn't have UniTask.IsCompleted)
        public static bool IsCompleted (this UniTask task) => task.IsCompleted;
        public static bool IsCompleted<T> (this UniTask<T> task) => task.IsCompleted;
        #else
        public static bool IsCompleted (this UniTask task) => task.Status.IsCompleted();
        public static bool IsCompleted<T> (this UniTask<T> task) => task.Status.IsCompleted();
        #endif
    }
}
