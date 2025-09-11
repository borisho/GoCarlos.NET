using GoCarlos.NET.Enums;
using GoCarlos.NET.Factories.Api;
using GoCarlos.NET.Services.Api;
using System.Data;

namespace GoCarlos.NET.Services.Impl;


/// <inheritdoc cref="IWindowService"/>

public sealed class WallListService(ILocalizerFactory localizerFactory) : IWallListService
{
    private readonly ILocalizerService localizerService = localizerFactory.GetByQualifier(LocalizerType.MainViewService);

    public DataTable Create()
    {
        var table = new DataTable();

        // Create fix columns
        DataColumn place = new()
        {
            DataType = typeof(int),
            ColumnName = localizerService["Place"]
        };

        DataColumn name = new()
        {
            DataType = typeof(string),
            ColumnName = localizerService["Name"]
        };

        DataColumn club = new()
        {
            DataType = typeof(string),
            ColumnName = localizerService["Club"]
        };

        DataColumn grade = new()
        {
            DataType = typeof(string),
            ColumnName = localizerService["Grade"]
        };

        DataColumn rating = new()
        {
            DataType = typeof(int),
            ColumnName = localizerService["Rating"]
        };


        table.Columns.Add(place);
        table.Columns.Add(name);
        table.Columns.Add(club);
        table.Columns.Add(grade);
        table.Columns.Add(rating);

        // Create dynamic columns

        // Fill with ScoredPlayers
        DataRow row1 = table.NewRow();
        row1[localizerService["Place"]] = 1;
        row1[localizerService["Name"]] = "Janko Novak";
        row1[localizerService["Club"]] = "Kosi";
        row1[localizerService["Grade"]] = "4d";
        row1[localizerService["Rating"]] = "2400";
        table.Rows.Add(row1);

        DataRow row2 = table.NewRow();
        row2[localizerService["Place"]] = 2;
        row2[localizerService["Name"]] = "Misko Novak";
        row2[localizerService["Club"]] = "Kosi";
        row2[localizerService["Grade"]] = "3d";
        row2[localizerService["Rating"]] = "2300";
        table.Rows.Add(row2);

        return table;
    }
}
