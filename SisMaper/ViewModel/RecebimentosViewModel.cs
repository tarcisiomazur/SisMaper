using Persistence;
using SisMaper.Models;
using System;
using System.Collections.ObjectModel;

namespace SisMaper.ViewModel
{
    public class RecebimentosViewModel : BaseViewModel, IRecebimento
    {
        public ObservableCollection<ViewListarFatura>? Faturas { get; private set; }

        private ViewListarFatura _faturaSelecionada;

        public ViewListarFatura FaturaSelecionada
        {
            get { return _faturaSelecionada; }
            set { SetField(ref _faturaSelecionada, value); }
        }


        public EditarFaturaCommand EditarFatura { get; private set; }
        public ExcluirFaturaCommand DeletarFatura { get; private set; }

        public Action OpenNovaFatura { get; set; }
        public Action<object?> OpenEditarFatura { get; set; }
        public Action FaturaExcluida { get; set; }

        private PersistenceContext persistenceContext;



        public RecebimentosViewModel()
        {
            //Faturas = DAO.FindWhereQuery<Fatura>("Id > 0");
            /*
            foreach(Fatura f in Faturas)
            {
                f?.Cliente?.Load();
            }
            */

            persistenceContext = new PersistenceContext();

            EditarFatura = new EditarFaturaCommand();
            DeletarFatura = new ExcluirFaturaCommand();
        }


        public void OpenCrudEditarFatura() => OpenEditarFatura?.Invoke(DAO.Load<Fatura>(FaturaSelecionada.Id));
        //public void OpenCrudEditarFatura() => Console.WriteLine(FaturaSelecionada.Id);


        public void ExcluirFatura()
        {
            //FaturaSelecionada.Delete();
        }


        public void Initialize(object? sender, EventArgs e)
        {
            Faturas = new ObservableCollection<ViewListarFatura>(View.Execute<ViewListarFatura>());
        }

    }




    public class EditarFaturaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            return vm.FaturaSelecionada is not null;
            //return true;
        }
        public override void Execute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            vm.OpenCrudEditarFatura();
        }
    }

    public class ExcluirFaturaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            return vm.FaturaSelecionada is not null;
        }
        public override void Execute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            vm.ExcluirFatura();
        }
    }


    public interface IRecebimento
    {
        public Action<object?> OpenEditarFatura { get; set; }
        public Action FaturaExcluida { get; set; }
    }
}
