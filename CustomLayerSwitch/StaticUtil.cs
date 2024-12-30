using System;
using System.Collections;
using UnityEngine;

namespace StudioCustomLayerSwitch
{
    // Token: 0x0200000A RID: 10
    public static class StaticUtils
    {
        // Token: 0x0600004D RID: 77 RVA: 0x0000392C File Offset: 0x00001B2C
        public static Coroutine DelayToDo(this MonoBehaviour mono, float delayTime, Action action, bool ignoreTimeScale = false)
        {
            Coroutine result;
            if (ignoreTimeScale)
            {
                result = mono.StartCoroutine(StaticUtils.DelayIgnoreTimeToDo(delayTime, action));
            }
            else
            {
                result = mono.StartCoroutine(StaticUtils.DelayToInvokeDo(delayTime, action));
            }
            return result;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x0000226B File Offset: 0x0000046B
        public static IEnumerator DelayToInvokeDo(float delaySeconds, Action action)
        {
            yield return new WaitForSeconds(delaySeconds);
            action();
            yield break;
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00002281 File Offset: 0x00000481
        public static IEnumerator DelayIgnoreTimeToDo(float delaySeconds, Action action)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + delaySeconds)
            {
                yield return null;
            }
            action();
            yield break;
        }
    }
}

