using System.Collections;
using NUnit.Framework;
using TurnTheTides;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class FloodTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void FloodTestSimplePasses()
    {

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator FloodTestWithEnumeratorPasses()
    {
        yield return new WaitForSeconds(1);

    }
}
