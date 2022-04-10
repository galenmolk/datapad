using UnityEngine;

public abstract class CommandArgumentParser
{
    public abstract bool CanParse(string text);
}

public abstract class CommandArgumentParser<T> : CommandArgumentParser
{
    public abstract T Value { get; }
}

public class StringCommandArgumentParser : CommandArgumentParser<string>
{
    public override string Value => m_value;
    private string m_value;

    public override bool CanParse(string text)
    {
        m_value = text;
        Debug.Log(m_value);
        return true;
    }
}
