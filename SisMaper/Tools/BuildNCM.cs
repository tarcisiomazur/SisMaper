using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Persistence;
using SisMaper.Models;

namespace SisMaper.Tools;

public class BuildNCM
{
    public static void Run()
    {
        var fileChoser = new OpenFileDialog
        {
            DefaultExt = ".tsv",
            Filter = "TSV Files (*.tsv)|*.tsv",
            Title = "Selecione o arquivo TSV disponibilizado",
            Multiselect = false
        };

        if (!fileChoser.ShowDialog().IsTrue()) return;

        var file = File.OpenText(fileChoser.FileName);

        var oldNCMs = DAO.All<NCM>();
        var newNCMs = new PList<NCM>();
        while (!file.EndOfStream)
        {
            var line = file.ReadLine();
            var columns = line.Split('\t');
            if (columns.Length < 3) continue;
            if (long.TryParse(columns[0], out var id))
            {
                newNCMs.Add(new NCM
                {
                    Id = id,
                    Descricao = columns[2]
                });
            }
        }

        Console.WriteLine(newNCMs.Max());


        var oldDict = CollectionExtension.ToDictionary(oldNCMs, ncm => ncm.Id, ncm => ncm.Descricao);
        var newDict = CollectionExtension.ToDictionary(newNCMs, ncm => ncm.Id, ncm => ncm.Descricao);

        oldNCMs.Where(pair => !newDict.ContainsKey(pair.Id)).All(ncm => ncm.Delete());
        newNCMs.RemoveAll(pair => oldDict.TryGetValue(pair.Id, out var des) && des == pair.Descricao);
        if (newNCMs.Save())
        {
            MessageBox.Show("A lista de NCMs foi atualizada com sucesso!", "Atualizar NCMs");
        }
        else
        {
            MessageBox.Show("Não foi possível atualizar a lista de NCMs!", "Erro ao Atualizar NCMs");
        }
    }
}