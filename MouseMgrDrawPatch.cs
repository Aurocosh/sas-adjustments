using HarmonyLib;
using ProjectMage.config;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SaltAndSacrificeAdjustments
{
    [HarmonyPatch]
    public static class MouseMgrDrawPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(MouseMgr), nameof(MouseMgr.Draw))]
        static IEnumerable<CodeInstruction> TranspileDraw(IEnumerable<CodeInstruction> instructions)
        {
            bool replaceCode = SaSAdjustments.MouseCursorInversionDisabled.Value;

            float firstTarget = -0.7853982f;
            float secondTarget = 1.570796f;
            float tolerance = 0.001f;

            var enumerator = instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CodeInstruction instruction = enumerator.Current;
                //myLogSource.LogInfo($"instruction: {instruction}");
                if (replaceCode && instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float instOperF && Math.Abs(firstTarget - instOperF) < tolerance)
                {
                    SaSAdjustments.MainLog.LogInfo($"Detected first target instruction for cursor inversion: {instruction}");

                    var nextInstructions = new List<CodeInstruction>(8);
                    for (int i = 0; i < 8; i++)
                    {
                        if (!enumerator.MoveNext())
                            break;
                        nextInstructions.Add(enumerator.Current);
                    }

                    bool isCorrectSequence = false;
                    if (nextInstructions.Count == 8)
                    {
                        var lastInstruction = nextInstructions[7];
                        isCorrectSequence = lastInstruction.opcode == OpCodes.Ldc_R4 && lastInstruction.operand is float lastOperF && Math.Abs(secondTarget - lastOperF) < tolerance;
                    }

                    if (isCorrectSequence)
                    {
                        SaSAdjustments.MainLog.LogInfo("Cursor inversion IL code detected successfully! Cursor inversion code removed!");
                        yield return instruction;
                        yield return nextInstructions[7];
                    }
                    else
                    {
                        SaSAdjustments.MainLog.LogInfo("Cursor inversion target was detected, but cursor inversion IL code sequene is incorrect. Original code might have changed!");
                        yield return instruction;
                        foreach (var tempInstruction in nextInstructions)
                            yield return tempInstruction;
                    }
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
