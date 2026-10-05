using System;

ElektricniAutomobil auto = new ElektricniAutomobil();
auto.Marka = "Tesla";

Console.WriteLine(auto.Marka);
Console.WriteLine(auto.Baterija);

auto.Pokreni();

public class Automobil
{
    public string Marka { get; set; } = "";

    public virtual void Pokreni()
    {
        Console.WriteLine("Pokrećem motor.");
    }
}

public class ElektricniAutomobil : Automobil
{
    public int Baterija { get; set; } = 100;

    public override void Pokreni()
    {
        Console.WriteLine("Pokrećem auto na struju");
    }
}


