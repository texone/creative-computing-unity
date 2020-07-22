using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCCommandLineArgs 
{
    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
    
    private readonly Dictionary<string, List<string>> _argumentMap = new Dictionary<string, List<string>>();

    private CCCommandLineArgs()
    {
        var args = System.Environment.GetCommandLineArgs();
        var currentKey = "";
        _argumentMap.Add(currentKey, new List<string>());
        foreach (var t in args)
        {
            if (t.StartsWith("-"))
            {
                currentKey = t.Substring(1);
                _argumentMap.Add(currentKey, new List<string>());
            }
            else
            {
                _argumentMap[currentKey].Add(t);
            }
        }
    }

    public int intArguemnt(string key = "", int defaultValue = 0, int index = 0)
    {
        if (!_argumentMap.ContainsKey(key)) return defaultValue;
        if (_argumentMap[key].Count <= index) return defaultValue;
        int.TryParse(_argumentMap[key][index], out defaultValue);
        return defaultValue;
    }

    private static CCCommandLineArgs _instance;

    public static CCCommandLineArgs Instance => _instance ?? (_instance = new CCCommandLineArgs());
    
}
