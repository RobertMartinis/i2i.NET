using ClosedXML;
using ClosedXML.Excel;
using i2i_dotnet.Features.TargetedTab.Models;


namespace i2i_dotnet.Features.TargetedTab.Services;

public class AnalyteFileService : IAnalyteFileService
{
    public IReadOnlyList<Analyte> LoadAnalytes(string path)
    { 
        var analytes = new List<Analyte>();
        var analyteList = new XLWorkbook(path);
        var ws = analyteList.Worksheet(1);
        foreach (var row in ws.RowsUsed())
        {
            double mz = row.Cell(1).GetDouble();
            string name = row.Cell(2).GetString();
            
            analytes.Add(new Analyte(name, mz));
        }
        return analytes;
    }
}