using Application.Request;
using Application.Response;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UsesCases
{
    public interface IPreferenciasUseCase
    {
        Task<PreferenciasResponse> SalvarPreferencias(QuestionarioPreferenciasRequest request);
        Task<PreferenciasResponse> ObterPreferencias(string usuarioId);
    }

    public class PreferenciasUseCase : IPreferenciasUseCase
    {
        private readonly IPreferenciasRepository _preferenciasRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PreferenciasUseCase(IPreferenciasRepository preferenciasRepository, IUsuarioRepository usuarioRepository)
        {
            _preferenciasRepository = preferenciasRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<PreferenciasResponse> SalvarPreferencias(QuestionarioPreferenciasRequest request)
        {
            // Validar dados
            ValidarQuestionario(request);

            var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var preferenciasExistentes = await _preferenciasRepository.GetByUsuarioIdAsync(request.UsuarioId);

            if (preferenciasExistentes != null)
            {
                // Atualizar preferências existentes
                AtualizarPreferencias(preferenciasExistentes, request);
                await _preferenciasRepository.UpdateAsync(preferenciasExistentes);
            }
            else
            {
                // Criar novas preferências
                var preferencias = new PreferenciasUsuarioDomain
                {
                    UsuarioId = request.UsuarioId,
                    HorarioFavorito = request.HorarioFavorito,
                    TipoComidaFavorito = request.TipoComidaFavorito,
                    NivelEstresse = request.NivelEstresse,
                    GostaViajar = request.GostaViajar,
                    PreferenciaLocal = request.PreferenciaLocal,
                    PreferenciaAmbiente = request.PreferenciaAmbiente,
                    ImportanciaEspiritualidade = request.ImportanciaEspiritualidade,
                    PosicaoPolitica = request.PosicaoPolitica,
                    Genero = request.Genero,
                    PreferenciaMusical = request.PreferenciaMusical,
                    MoodFilmesSeries = request.MoodFilmesSeries,
                    StatusRelacionamento = request.StatusRelacionamento,
                    TemFilhos = request.TemFilhos,
                    PreferenciaAnimal = request.PreferenciaAnimal,
                    FraseDefinicao = request.FraseDefinicao
                };

                await _preferenciasRepository.AddAsync(preferencias);
            }

            return new PreferenciasResponse
            {
                UsuarioId = request.UsuarioId,
                Status = "preferencias_salvas",
                DataAtualizacao = DateTime.UtcNow,
                HorarioFavorito = request.HorarioFavorito,
                TipoComidaFavorito = request.TipoComidaFavorito,
                PreferenciaMusical = request.PreferenciaMusical
            };
        }

        public async Task<PreferenciasResponse> ObterPreferencias(string usuarioId)
        {
            var preferencias = await _preferenciasRepository.GetByUsuarioIdAsync(usuarioId);
            if (preferencias == null)
                throw new Exception("Preferências não encontradas");

            return new PreferenciasResponse
            {
                UsuarioId = preferencias.UsuarioId,
                Status = "preferencias_encontradas",
                DataAtualizacao = preferencias.DataAtualizacao,
                HorarioFavorito = preferencias.HorarioFavorito,
                TipoComidaFavorito = preferencias.TipoComidaFavorito,
                PreferenciaMusical = preferencias.PreferenciaMusical
            };
        }

        private void ValidarQuestionario(QuestionarioPreferenciasRequest request)
        {
            // Validar horário favorito
            var horariosValidos = new[] { "Manhã", "Tarde", "Noite", "Madrugada" };
            if (!horariosValidos.Contains(request.HorarioFavorito))
                throw new Exception("Horário favorito inválido");

            // Validar nível de estresse
            if (request.NivelEstresse < 1 || request.NivelEstresse > 10)
                throw new Exception("Nível de estresse deve ser entre 1 e 10");

            // Validar importância espiritualidade
            if (request.ImportanciaEspiritualidade < 1 || request.ImportanciaEspiritualidade > 10)
                throw new Exception("Importância da espiritualidade deve ser entre 1 e 10");

            // Validar posição política
            var posicoesValidas = new[] { "Direita", "Centro-Direita", "Esquerda", "Centro-Esquerda", "Apolítico" };
            if (!posicoesValidas.Contains(request.PosicaoPolitica))
                throw new Exception("Posição política inválida");

            // Validar gênero
            var generosValidos = new[] { "Mulher", "Homem", "Não-Binário" };
            if (!generosValidos.Contains(request.Genero))
                throw new Exception("Gênero inválido");
        }

        private void AtualizarPreferencias(PreferenciasUsuarioDomain  preferencias, QuestionarioPreferenciasRequest request)
        {
            preferencias.HorarioFavorito = request.HorarioFavorito;
            preferencias.TipoComidaFavorito = request.TipoComidaFavorito;
            preferencias.NivelEstresse = request.NivelEstresse;
            preferencias.GostaViajar = request.GostaViajar;
            preferencias.PreferenciaLocal = request.PreferenciaLocal;
            preferencias.PreferenciaAmbiente = request.PreferenciaAmbiente;
            preferencias.ImportanciaEspiritualidade = request.ImportanciaEspiritualidade;
            preferencias.PosicaoPolitica = request.PosicaoPolitica;
            preferencias.Genero = request.Genero;
            preferencias.PreferenciaMusical = request.PreferenciaMusical;
            preferencias.MoodFilmesSeries = request.MoodFilmesSeries;
            preferencias.StatusRelacionamento = request.StatusRelacionamento;
            preferencias.TemFilhos = request.TemFilhos;
            preferencias.PreferenciaAnimal = request.PreferenciaAnimal;
            preferencias.FraseDefinicao = request.FraseDefinicao;
            preferencias.DataAtualizacao = DateTime.UtcNow;
        }
    }
}
