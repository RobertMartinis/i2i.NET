namespace i2i_dotnet.Features.TargetedTab.Model;

public class Analyte
{
    public string Name {get;}
    public double Mz { get; }
    
    public Analyte(string name, double mz)
        {
        Name = name;
        Mz = mz;
        }
}