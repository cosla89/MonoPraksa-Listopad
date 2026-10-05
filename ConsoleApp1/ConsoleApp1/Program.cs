using System;

Građevina građevina = new Građevina();  
Zgrada zgrada = new Zgrada();
zgrada.Adresa = "Antuna Branka Šimića 10";
IInformacije informacije = zgrada;
informacije.info();
Console.WriteLine("Količina: " + građevina.Kolicina);   


public interface IInformacije
{
    void info();
}
class Građevina : IInformacije
{
    public double cijena = 10000;
    private int _kolicina = 1;
    public int Kolicina
    {
        get { return _kolicina; }
    }
    public virtual void info()
    {
        Console.WriteLine("Cijena: " + cijena);
    }

}
class Zgrada : Građevina
{
    public string Adresa { get; set; }
    public override void info()
    {
        Console.WriteLine("Adresa: " + Adresa);
    }
}


