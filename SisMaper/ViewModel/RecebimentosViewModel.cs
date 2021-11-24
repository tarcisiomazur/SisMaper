using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisMaper.ViewModel
{
    public class RecebimentosViewModel : BaseViewModel, IFaturas
    {
        public PList<Fatura> Faturas { get; private set; }

        private Fatura _faturaSelecionada;

        public Fatura FaturaSelecionada
        {
            get { return _faturaSelecionada; }
            set { SetField(ref _faturaSelecionada, value); }
        }


        public NovaFaturaCommand NovaFatura { get; private set; }
        public EditarFaturaCommand EditarFatura { get; private set; }
        public ExcluirFaturaCommand DeletarFatura { get; private set; }

        public Action OpenNovaFatura { get; set; }
        public Action OpenEditarFatura { get; set; }
        public Action FaturaExcluida { get; set; }

        public RecebimentosViewModel()
        {
            Faturas = DAO.FindWhereQuery<Fatura>("Id > 0");

            NovaFatura = new NovaFaturaCommand();
            EditarFatura = new EditarFaturaCommand();
            DeletarFatura = new ExcluirFaturaCommand();
        }



        public void OpenCrudNovaFatura() => OpenNovaFatura?.Invoke();

        public void OpenCrudEditarFatura() => OpenEditarFatura?.Invoke();


        public void ExcluirFatura()
        {
            //FaturaSelecionada.Delete();
        }

    }



    public class NovaFaturaCommand : BaseCommand
    {
        public override void Execute(object parameter)
        {
            RecebimentosViewModel vm = (RecebimentosViewModel)parameter;
            vm.OpenCrudNovaFatura();
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


    public interface IFaturas
    {
        public Action OpenNovaFatura { get; set; }
        public Action OpenEditarFatura { get; set; }
        public Action FaturaExcluida { get; set; }
    }
}
