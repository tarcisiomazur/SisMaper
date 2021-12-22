using System;
using SisMaper.Models;
using SisMaper.Tools;

namespace SisMaper.ViewModel;

public class ConfiguracoesViewModel : BaseViewModel
{
    #region Actions

    public event Action? Cancel;

    public event Action? Save;

    #endregion

    #region Properties

    public Configuracoes Empresa { get; set; }

    public string ConsumerKey { get; set; }

    public string ConsumerSecret { get; set; }

    public string Token { get; set; }

    public string TokenSecret { get; set; }

    #endregion

    #region ICommands

    public SimpleCommand SaveCmd => new(Salvar);

    #endregion

    private void Salvar()
    {
        if (!Empresa.CNPJ.IsCnpj())
        {
            OnShowMessage("Erro ao Salvar Empresa", "CNPJ Inválido");
            return;
        }

        try
        {
            Empresa.CONSUMER_KEY = Encrypt.RSAEncryption(ConsumerKey);
            Empresa.CONSUMER_SECRET = Encrypt.RSAEncryption(ConsumerSecret);
            Empresa.ACCESS_TOKEN = Encrypt.RSAEncryption(Token);
            Empresa.ACCESS_TOKEN_SECRET = Encrypt.RSAEncryption(TokenSecret);
            Empresa.Save();
        }
        catch
        {
            OnShowMessage("Erro de Encriptação", "Erro ao Encriptar os Dados");
        }
    }

    public void Initialize(Configuracoes empresa)
    {
        Empresa = empresa;
        try
        {
            ConsumerKey = Encrypt.RSADecryption(Empresa.CONSUMER_KEY);
            ConsumerSecret = Encrypt.RSADecryption(Empresa.CONSUMER_SECRET);
            Token = Encrypt.RSADecryption(Empresa.ACCESS_TOKEN);
            TokenSecret = Encrypt.RSADecryption(Empresa.ACCESS_TOKEN_SECRET);
        }
        catch
        {
            OnShowMessage("Erro de Desencriptação", "Erro ao desencriptar os Dados");
        }
    }
}