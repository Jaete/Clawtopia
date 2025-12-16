using Godot;

public class Counter
{
    public string Name { get; private set; }
    public int Value { get; private set; }

    public Counter(string name, int initialValue)
    {
        Name = name;
        Value = initialValue;
    }

    public void Add(int amount) => Value += amount;

    public void Remove(int amount) => Value = Mathf.Max(0, Value - amount);

    public void SetValue(int newValue) => Value = newValue;
}