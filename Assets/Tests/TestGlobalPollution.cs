using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.TestTools;

public class TestGlobalPollution
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestGetPollutionEffect()
    {
        //Should return the pollutionEffect
    }

    [Test]
    public void TestIncreasePollutionEffect()
    {
        //Sea levels should rise
    }

    [Test]
    public void TestDecreasePollutionEffect()
    {
        //Sea levels should decrease
    }

    [Test]
    public void TestMaxPollutionEffect()
    {
        //Raise a GameOver exception
    }

    [Test]
    public void TestIncreasePollutionTotal()
    {
        //pollutionEffect should increase x amount on turn end
        //Global pollutionEffect should not increase more than x “steps”

    }

    [Test]
    public void TestDecreasePollutionTotal()
    {
        //pollutionEffect should decrease x amount on turn end
        //Global pollutionEffect should not decrease more than x “steps”

    }
}
