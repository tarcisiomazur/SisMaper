using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisMaper.ViewModel
{
    public class CrudPessoaFisicaViewModel : BaseViewModel
    {
        private Estado _estadoSelecionado;

        public Estado EstadoSelecionado
        {
            get { return _estadoSelecionado; }
            set { SetField(ref _estadoSelecionado, value); }
        }


        private Cidade _cidadeSelecionada;

        public Cidade CidadeSelecionada
        {
            get { return _cidadeSelecionada; }
            set { SetField(ref _cidadeSelecionada, value); }
        }


    }
}
