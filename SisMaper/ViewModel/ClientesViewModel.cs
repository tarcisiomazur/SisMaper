using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisMaper.ViewModel
{
    public class ClientesViewModel : BaseViewModel
    {
        private Cliente _clienteSelecionado;

        public Cliente ClienteSelecionado
        {
            get { return _clienteSelecionado; }
            set { SetField(ref _clienteSelecionado, value); }
        }

        public PList<PessoaFisica> PessoaFisicaList { get; private set; }
        public PList<PessoaJuridica> PessoaJuridicaList { get; private set; }


        public ClientesViewModel()
        {
            PessoaFisicaList = DAO.FindWhereQuery<PessoaFisica>("Cliente_Id > 0");
            PessoaJuridicaList = DAO.FindWhereQuery<PessoaJuridica>("Cliente_Id > 0");

            ClienteSelecionado = null;
        }
    }
}
