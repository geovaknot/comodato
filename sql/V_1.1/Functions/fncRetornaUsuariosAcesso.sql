GO
/****** Object:  UserDefinedFunction [dbo].[fncRetornaUsuariosAcesso]    Script Date: 06/04/2022 10:26:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[fncRetornaUsuariosAcesso]
(
	@p_nidUsuario numeric
)
RETURNS 
	@tbReturn TABLE
		(
			nidUsuario numeric
		)
	AS
	BEGIN 
		declare @vccdPerfil numeric

		-- Usuário Com Perfil
		select @p_nidUsuario = TbUsuario.nidUsuario, @vccdPerfil = MIN(tbPerfil.ccdPerfil)
		From TbUsuario
		inner join tbUsuarioPerfil 
			on tbUsuarioPerfil.nidUsuario = TbUsuario.nidUsuario 
			and tbUsuarioPerfil.bidAtivo=1
		inner join tbPerfil 
			on tbPerfil.nidPerfil = tbUsuarioPerfil.nidPerfil
			and tbPerfil.bidAtivo = 1
		where TbUsuario.bidAtivo = 1
		and TbUsuario.nidUsuario = @p_nidUsuario
		AND TbUsuario.nidUsuario IS NOT NULL
		GROUP BY TbUsuario.nidUsuario

		IF (@vccdPerfil IN (1, 5, 9, 6) ) --(1) Administrador 3M/ (5) Assistência Técnica 3M / (9) Controle de Estoque
			BEGIN 
				INSERT INTO @tbReturn
				SELECT nidUsuario FROM tbUsuario where bidAtivo = 1
				AND TbUsuario.nidUsuario IS NOT NULL
			END
		--ELSE IF (@vccdPerfil = 6) -- (6) Líder Empresa Técnica
		--	BEGIN
		--		INSERT INTO @tbReturn
		--		SELECT DISTINCT TbUsuariosTecnco.ID_USUARIO nidUsuario
		--		FROM (
		--				SELECT nidUsuario ID_USUARIO FROM tbUsuario
		--				 WHERE tbUsuario.nidUsuario= @p_nidUsuario AND bidAtivo = 1
		--					UNION ALL 
		--				SELECT ID_USUARIO FROM TB_TECNICO WHERE TB_TECNICO.ID_USUARIO_COORDENADOR = @p_nidUsuario  AND FL_ATIVO = 'S'
		--			) TbUsuariosTecnco WHERE TbUsuariosTecnco.ID_USUARIO IS NOT NULL
		--	END 
		ELSE IF (@vccdPerfil = 2) -- (2) Técnico 3M (similar a líder téc. externo)
			BEGIN
				INSERT INTO @tbReturn
				SELECT DISTINCT TbUsuariosTecnco.ID_USUARIO nidUsuario
				FROM (
						SELECT nidUsuario ID_USUARIO FROM tbUsuario
						 WHERE tbUsuario.nidUsuario= @p_nidUsuario AND bidAtivo = 1
							UNION ALL 
						SELECT ID_USUARIO FROM TB_TECNICO WHERE FL_ATIVO = 'S' --TB_TECNICO.ID_USUARIO_TECNICOREGIONAL = @p_nidUsuario  AND FL_ATIVO = 'S' (AMS_SL00044005)
						--AND TP_TECNICO = 'T'
					) TbUsuariosTecnco WHERE TbUsuariosTecnco.ID_USUARIO IS NOT NULL
			END 
		ELSE IF (@vccdPerfil = 10) -- (10) Ger. Regional
			BEGIN
				INSERT INTO @tbReturn
				SELECT DISTINCT TbUsuariosVend.ID_USUARIO nidUsuario
				FROM (
						SELECT nidUsuario ID_USUARIO FROM tbUsuario
						 WHERE tbUsuario.nidUsuario= @p_nidUsuario AND bidAtivo = 1
						 AND TbUsuario.nidUsuario IS NOT NULL
							UNION ALL 
						SELECT ID_USUARIO FROM TB_VENDEDOR WHERE TB_VENDEDOR.ID_USUARIO_REGIONAL = @p_nidUsuario  AND FL_ATIVO = 'S'
					) TbUsuariosVend
			END
		ELSE IF (@vccdPerfil = 11) -- (11) Ger. Nacional
			BEGIN
				INSERT INTO @tbReturn
				SELECT DISTINCT TbUsuariosVend.ID_USUARIO nidUsuario
				FROM (
						SELECT nidUsuario ID_USUARIO FROM tbUsuario
						 WHERE tbUsuario.nidUsuario= @p_nidUsuario AND bidAtivo = 1
						 AND TbUsuario.nidUsuario IS NOT NULL
							UNION ALL 
						SELECT ID_USUARIO FROM TB_VENDEDOR WHERE FL_ATIVO = 'S'
						AND ID_USUARIO IS NOT NULL
					) TbUsuariosVend
			END
		ELSE
			BEGIN 
				INSERT INTO @tbReturn
				SELECT @p_nidUsuario nidUsuario
			END

		return 
	END

