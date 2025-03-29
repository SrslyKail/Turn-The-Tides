using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGlobalPollution
{
    /// <summary>
    /// Test the current pollutionEffects are working.
    /// </summary>
    [Test]
    public void TestGetPollutionEffect()
    {
        //Should return the pollutionEffect
    }

    /// <summary>
    /// Test that increasing the pollutionEffect level causes the expected changes.
    /// </summary>
    [Test]
    public void TestIncreasePollutionEffect()
    {
        //Sea levels should rise
    }

    /// <summary>
    /// Test that decreasing the pollutionEffect level causes the expected changes.
    /// </summary>
    [Test]
    public void TestDecreasePollutionEffect()
    {
        //Sea levels should decrease
    }


    /// <summary>
    /// Test that increasing the pollutionEffect level to max causes a game over.
    /// </summary>
    [Test]
    public void TestMaxPollutionEffect()
    {
        //Raise a GameOver exception
    }

    /// <summary>
    /// Test that increasing the pollutionTotal doesnt instantly cause the pollutionEffect to increase by the same amount.
    /// </summary>
    [Test]
    public void TestIncreasePollutionTotal()
    {
        //pollutionEffect should increase x amount on turn end
        //Global pollutionEffect should not increase more than x “steps”

    }


    /// <summary>
    /// Test that decreasing the pollutionTotal doesnt instantly cause the pollutionEffect to decrease by the same amount.
    /// </summary>
    [Test]
    public void TestDecreasePollutionTotal()
    {
        //pollutionEffect should decrease x amount on turn end
        //Global pollutionEffect should not decrease more than x “steps”

    }
}
