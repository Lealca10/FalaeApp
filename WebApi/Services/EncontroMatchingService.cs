using Application.response;
using Domain.Entities;
using Infrastructure.BaseDados;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.Request;

namespace WebApi.Services
{
    public class EncontroMatchingService : IEncontroMatchingService
    {
        private readonly DatabaseContext _context;

        public EncontroMatchingService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<MatchingResult> EncontrarParticipantesCompatíveis(MatchingRequest request)
        {
            try
            {
                // Buscar usuários ativos com preferências
                var usuariosComPreferencias = await _context.Usuarios
                    .Include(u => u.Preferencias)
                    .Where(u => u.Ativo && u.Preferencias != null)
                    .ToListAsync();

                if (usuariosComPreferencias.Count < request.NumeroParticipantes)
                {
                    return new MatchingResult
                    {
                        Sucesso = false,
                        Mensagem = $"Não há usuários suficientes. Necessário: {request.NumeroParticipantes}, Disponíveis: {usuariosComPreferencias.Count}"
                    };
                }

                // Algoritmo de matching otimizado
                var melhorGrupo = await EncontrarMelhorGrupo(usuariosComPreferencias, request);

                if (melhorGrupo.Count == request.NumeroParticipantes)
                {
                    var compatibilidadeMedia = CalcularCompatibilidadeMedia(melhorGrupo);

                    return new MatchingResult
                    {
                        Sucesso = true,
                        Mensagem = "Grupo compatível encontrado!",
                        ParticipantesSugeridos = melhorGrupo.Select(u => new UsuarioInfo
                        {
                            Id = u.Id,
                            Nome = u.Nome,
                            Email = u.Email,
                            Cidade = u.Cidade
                        }).ToList(),
                        PreferenciasCompatíveis = compatibilidadeMedia
                    };
                }

                return new MatchingResult
                {
                    Sucesso = false,
                    Mensagem = "Não foi possível encontrar um grupo com a compatibilidade mínima requerida"
                };
            }
            catch (Exception ex)
            {
                return new MatchingResult
                {
                    Sucesso = false,
                    Mensagem = $"Erro no matching: {ex.Message}"
                };
            }
        }

        private async Task<List<UsuarioDomain>> EncontrarMelhorGrupo(List<UsuarioDomain> usuarios, MatchingRequest request)
        {
            var melhorGrupo = new List<UsuarioDomain>();
            var melhorCompatibilidade = 0;

            // Tentar diferentes combinações (limitado para performance)
            for (int i = 0; i < Math.Min(usuarios.Count, 10); i++) // Testa no máximo 10 usuários como base
            {
                var usuarioBase = usuarios[i];
                var grupo = new List<UsuarioDomain> { usuarioBase };

                foreach (var outroUsuario in usuarios.Where(u => u.Id != usuarioBase.Id))
                {
                    if (grupo.Count >= request.NumeroParticipantes) break;

                    var compativelComTodos = true;
                    foreach (var usuarioNoGrupo in grupo)
                    {
                        var compatibilidade = CalcularPreferenciasCompatíveis(
                            usuarioNoGrupo.Preferencias,
                            outroUsuario.Preferencias
                        );

                        if (compatibilidade < request.MinimoPreferenciasIguais)
                        {
                            compativelComTodos = false;
                            break;
                        }
                    }

                    if (compativelComTodos)
                    {
                        grupo.Add(outroUsuario);
                    }
                }

                if (grupo.Count == request.NumeroParticipantes)
                {
                    var compatibilidadeMedia = CalcularCompatibilidadeMedia(grupo);
                    if (compatibilidadeMedia > melhorCompatibilidade)
                    {
                        melhorCompatibilidade = compatibilidadeMedia;
                        melhorGrupo = grupo;
                    }
                }
            }

            return melhorGrupo;
        }

        public int CalcularPreferenciasCompatíveis(PreferenciasUsuarioDomain pref1, PreferenciasUsuarioDomain pref2)
        {
            if (pref1 == null || pref2 == null) return 0;

            var preferenciasIguais = 0;

            // Comparar preferências básicas (11 critérios)
            if (!string.IsNullOrEmpty(pref1.HorarioFavorito) && pref1.HorarioFavorito == pref2.HorarioFavorito) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.TipoComidaFavorito) && pref1.TipoComidaFavorito == pref2.TipoComidaFavorito) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.PreferenciaLocal) && pref1.PreferenciaLocal == pref2.PreferenciaLocal) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.PreferenciaAmbiente) && pref1.PreferenciaAmbiente == pref2.PreferenciaAmbiente) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.PosicaoPolitica) && pref1.PosicaoPolitica == pref2.PosicaoPolitica) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.Genero) && pref1.Genero == pref2.Genero) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.PreferenciaMusical) && pref1.PreferenciaMusical == pref2.PreferenciaMusical) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.MoodFilmesSeries) && pref1.MoodFilmesSeries == pref2.MoodFilmesSeries) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.StatusRelacionamento) && pref1.StatusRelacionamento == pref2.StatusRelacionamento) preferenciasIguais++;
            if (!string.IsNullOrEmpty(pref1.PreferenciaAnimal) && pref1.PreferenciaAnimal == pref2.PreferenciaAnimal) preferenciasIguais++;

            // Comparar nível de estresse (tolerância de ±2)
            if (Math.Abs(pref1.NivelEstresse - pref2.NivelEstresse) <= 2) preferenciasIguais++;

            // Comparar importância espiritualidade (tolerância de ±2)
            if (Math.Abs(pref1.ImportanciaEspiritualidade - pref2.ImportanciaEspiritualidade) <= 2) preferenciasIguais++;

            return preferenciasIguais;
        }

        private int CalcularCompatibilidadeMedia(List<UsuarioDomain> grupo)
        {
            var totalCompatibilidade = 0;
            var comparacoes = 0;

            for (int i = 0; i < grupo.Count; i++)
            {
                for (int j = i + 1; j < grupo.Count; j++)
                {
                    var compatibilidade = CalcularPreferenciasCompatíveis(
                        grupo[i].Preferencias,
                        grupo[j].Preferencias
                    );
                    totalCompatibilidade += compatibilidade;
                    comparacoes++;
                }
            }

            return comparacoes > 0 ? totalCompatibilidade / comparacoes : 0;
        }
    }
}