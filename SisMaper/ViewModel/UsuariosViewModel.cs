using Persistence;
using SisMaper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SisMaper.Tools;

namespace SisMaper.ViewModel
{
    public class UsuariosViewModel : BaseViewModel
    {

        public PList<Usuario>? Usuarios { get; set; }
        public IEnumerable<Usuario>? UsuariosFiltrados { get; set; }

        public string TextoFiltro { get; set; }

        public Usuario? UsuarioSelecionado { get; set; }


        public SimpleCommand NovoUsuarioCmd => new(() => OpenCrudUsuario?.Invoke(new CrudUsuarioViewModel(true, 0, Usuarios)));
        public SimpleCommand EditarUsuarioCmd => new(() => OpenCrudUsuario?.Invoke(new CrudUsuarioViewModel(false, UsuarioSelecionado.Id, Usuarios)), () => UsuarioSelecionado is not null);


        public Action<CrudUsuarioViewModel>? OpenCrudUsuario;

        public UsuariosViewModel()
        {
            Usuarios = DAO.All<Usuario>();
            UsuariosFiltrados = Usuarios;
            PropertyChanged += UdpateFilter;
        }

        private void UdpateFilter(object? sender, PropertyChangedEventArgs e)
        {
            if (Usuarios != null && e.PropertyName is nameof(TextoFiltro))
            {
                UsuariosFiltrados = Usuarios.Where(u => u.Nome.Contains(TextoFiltro, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public void Initialize(object? sender, EventArgs e)
        {
            Usuarios = DAO.All<Usuario>();
            UsuariosFiltrados = Usuarios;
            UsuarioSelecionado = null;
        }

        public void Clear(object? sender, EventArgs e)
        {
            Usuarios = null;
            UsuariosFiltrados = null;
        }









        public class CrudUsuarioViewModel : BaseViewModel
        {

            public bool PermissaoCadastro { get; set; }
            public bool PermissaoVenda { get; set; }
            public bool PermissaoRecebimento { get; set; }

            private bool _permissaoAdmin;
            public bool PermissaoAdmin
            {
                get { return _permissaoAdmin; }
                set
                {
                    if (value)
                    {
                        PermissaoCadastro = PermissaoVenda = PermissaoRecebimento = true;
                    }
                    _permissaoAdmin = value;
                }
            }

            private Usuario.Tipo_Permissao permissao = Usuario.Tipo_Permissao.Nenhum;


            private IEnumerable<Usuario>? usuarios;
            public Usuario? Usuario { get; set; }


            public bool IsNovoUsuario { get; private set; }
            public bool IsUserAdmin => Main.Usuario.Permissao.HasFlag(Usuario.Tipo_Permissao.Gerenciamento);


            public string Login { get; set; }
            public string SenhaAtual { get; set; }
            public string NovaSenha { get; set; }


            public SimpleCommand SalvarUsuarioCmd => new(SalvarUsuario);
            public Action? UsuarioSaved { get; set; }

            public CrudUsuarioViewModel(bool isNovoUsuario, long usuarioId, IEnumerable<Usuario>? usuarios)
            {
                IsNovoUsuario = isNovoUsuario;
                this.usuarios = usuarios;

                if (IsNovoUsuario)
                {
                    Usuario = new Usuario();
                }

                else
                {
                    Usuario = DAO.Load<Usuario>(usuarioId);
                    if (Usuario is null) return;

                    PermissaoAdmin = Usuario.Permissao.HasFlag(Usuario.Tipo_Permissao.Gerenciamento);
                    PermissaoCadastro = Usuario.Permissao.HasFlag(Usuario.Tipo_Permissao.Cadastros);
                    PermissaoVenda = Usuario.Permissao.HasFlag(Usuario.Tipo_Permissao.Venda);
                    PermissaoRecebimento = Usuario.Permissao.HasFlag(Usuario.Tipo_Permissao.Recebimento);
                }

            }


            //retorna se achou um nome ou um login igual
            private bool? SearchNomeAndLogin()
            {
                if (usuarios is null || Usuario is null) return null;

                foreach (Usuario u in usuarios)
                {
                    if (u.Id != Usuario.Id && u.Nome.Equals(Usuario.Nome))
                    {
                        OnShowMessage("Erro ao salvar usuário", "Nome já existe");
                        return true;
                    }

                    if (u.Id != Usuario.Id && u.Login.Equals(Usuario.Login))
                    {
                        OnShowMessage("Erro ao salvar usuário", "Login já existe");
                        return true;
                    }
                }

                return false;
            }

            //retorna se as permissões estão ok
            private bool? CheckPermissoes()
            {
                if (Usuario is null || usuarios is null) return null;

                if (PermissaoAdmin)
                {
                    Usuario.Permissao = Usuario.Tipo_Permissao.All;
                    return true;
                }

                if (PermissaoCadastro) permissao |= Usuario.Tipo_Permissao.Cadastros;

                if (PermissaoVenda) permissao |= Usuario.Tipo_Permissao.Venda;

                if (PermissaoRecebimento) permissao |= Usuario.Tipo_Permissao.Recebimento;


                if (permissao == Usuario.Tipo_Permissao.Nenhum)
                {
                    OnShowMessage("Erro ao salvar usuário", "O usuário deve ter permissões");
                    return false;
                }

                //se não tem nenhum outro admin, o admin não pode tirar a própria permissão
                if (!permissao.HasFlag(Usuario.Tipo_Permissao.Gerenciamento) && !usuarios.Any(u => (u.Id != Usuario.Id && u.Permissao.HasFlag(Usuario.Tipo_Permissao.Gerenciamento))))
                {
                    OnShowMessage("Erro ao salvar usuário", "Deve existir um administrador");
                    return false;
                }

                Usuario.Permissao = permissao;
                return true;

            }

            //retorna se a senha está ok
            private bool? CheckSenha()
            {

                if (Usuario is null) return null;

                bool senhaAtualVazia = SenhaAtual.Equals(Encrypt.ToSha512(""));
                bool novaSenhaVazia = NovaSenha.Equals(Encrypt.ToSha512(""));

                // se for um novo usuario
                if (IsNovoUsuario)
                {
                    if (novaSenhaVazia)
                    {
                        OnShowMessage("Erro ao salvar usuário", "Senha Inválida");
                        return false;
                    }

                    Usuario.Senha = NovaSenha;
                    return true;
                }


                //se for editar um usuario

                if (!IsUserAdmin)
                {
                    Console.WriteLine($"LOGIN ==> {Login}           SENHA ==> {senhaAtualVazia}");

                    if (string.IsNullOrWhiteSpace(Login) || senhaAtualVazia)
                    {
                        if (!novaSenhaVazia)
                        {
                            OnShowMessage("Erro ao salvar usuário", "Você deve informar a seu login e senha");
                            return false;
                        }
                        return true;
                    }

                    if (SenhaAtual != Usuario.Senha || Login != Usuario.Login)
                    {
                        OnShowMessage("Erro ao salvar usuário", "Login ou senha incorreto");
                        return false;
                    }

                    if (novaSenhaVazia)
                    {
                        OnShowMessage("Erro ao salvar usuário", "Nova Senha Inválida");
                        return false;
                    }

                    Usuario.Senha = NovaSenha;
                    return true;

                }

                //caso seja o admin, só precisa da nova senha

                if (novaSenhaVazia)
                    return true;

                Usuario.Senha = NovaSenha;
                return true;


            }


            private void SalvarUsuario()
            {
                if (Usuario is null) return;

                if (string.IsNullOrWhiteSpace(Usuario.Nome))
                {
                    OnShowMessage("Erro ao salvar usuário", "Nome Inválido");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Usuario.Login))
                {
                    OnShowMessage("Erro ao salvar usuário", "Login Inválido");
                    return;
                }

                if (SearchNomeAndLogin() != false || CheckPermissoes() != true || CheckSenha() != true) return;


                try
                {
                    if (Usuario.Save())
                    {
                        OnShowMessage("Salvar Usuário", "Usuário salvo com sucesso");
                        UsuarioSaved?.Invoke();
                    }

                }
                catch
                {
                    OnShowMessage("Salvar Usuário", "Não foi possível salvar o usuário");
                }

            }
        }
    }
}
