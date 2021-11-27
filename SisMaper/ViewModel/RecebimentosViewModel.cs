using Persistence;
using SisMaper.Models;
using System;

namespace SisMaper.ViewModel
{
    public class RecebimentosViewModel : BaseViewModel, IRecebimento
    {
        public PList<Fatura> Faturas { get; private set; }

        private Fatura _faturaSelecionada;

        public Fatura FaturaSelecionada
        {
            get { return _faturaSelecionada; }
            set { SetField(ref _faturaSelecionada, value); }
        }


        public EditarFaturaCommand EditarFatura { get; private set; }
        public ExcluirFaturaCommand DeletarFatura { get; private set; }

        public Action OpenNovaFatura { get; set; }
        public Action OpenEditarFatura { get; set; }
        public Action FaturaExcluida { get; set; }

        public RecebimentosViewModel()
        {
            Faturas = DAO.FindWhereQuery<Fatura>("Id > 0");

            foreach(Fatura f in Faturas)
            {
                f?.Cliente?.Load();
            }

            EditarFatura = new EditarFaturaCommand();
            DeletarFatura = new ExcluirFaturaCommand();
        }


        public void OpenCrudEditarFatura() => OpenEditarFatura?.Invoke();


        public void ExcluirFatura()
        {
            //FaturaSelecionada.Delete();
        }

    }




    public class EditarFaturaCommand : BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            return vm.FaturaSelecionada is not null;
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
        public Action OpenEditarFatura { get; set; }
        public Action FaturaExcluida { get; set; }
    }
}
