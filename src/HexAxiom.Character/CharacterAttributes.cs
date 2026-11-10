namespace HexAxiom.Character;

internal sealed class CharacterAttributes
{
    public int Body { get; set; }
    public int Stun { get; set; }
    public int Endurance { get; set; }
    public int Attributes { get; set; } ///look up here for the attribute values
    public int Skills { get; set; } ///look up here for the skill values

}

internal sealed class IntResourceStat
{
    public int Current { get; private set; }
    public int Maximum { get; private set; }

    public IntResourceStat(int current, int maximum)
    {
        Current = current;
        Maximum = maximum;
    }

    public void Update(int current)
    {
        Current = Math.Clamp(current, 0, Maximum);
    }

    public void Decrease(int amount)
    {
        Current = Math.Max(Current - amount, 0);
    }

    public void Increase(int amount)
    {
        Current = Math.Min(Current + amount, Maximum);
    }

    public void UpdateMaximum(int maximum)
    {
        Maximum = Math.Max(maximum, 0);
        Current = Math.Clamp(Current, 0, Maximum);
    }

    public void Reset()
    {
        Current = Maximum;
    }

    public bool IsDepleted => Current <= 0;

    public override string ToString()
    {
        return $"{Current}/{Maximum}";
    }

}

public sealed class AttributeStat
{
    public int Value { get; private set; }

    public int TempValue { get; private set; }

    public AttributeStat(int value)
    {
        Value = value;

        TempValue = value;
    }

    public void Update(int value)
    {
        Value = Math.Max(value, 0);
    }

    public void Increase(int amount) ///Permanent increase to the attribute value
    {
        Value = Math.Max(Value + amount, 0);
    }

    public void Decrease(int amount) ///Permanent decrease to the attribute value
    {
        Value = Math.Max(Value - amount, 0);
    }


    public void Buff(int amount)
    {
        TempValue = Math.Max(TempValue + amount, 0);
    }


    public void Debuff(int amount)
    {
        TempValue = Math.Max(TempValue - amount, 0);
    }

    public void Reset()
    {
        TempValue = Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public int GetCurrentValue()
    {
        return TempValue;
    }


}