GO
/****** Object:  StoredProcedure [dbo].[prcClienteComboSelect]    Script Date: 15/09/2023 12:38:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--select * from TB_TECNICO where ID_USUARIO = 35
--select distinct CD_CLIENTE from TB_TECNICO_CLIENTE where CD_TECNICO = '347830' and cd_ordem < 2
--select * from tbUsuario where cnmNome like '%adeilton%'

-- =============================================
-- Author:		Paulo Rabelo
-- Create date: 09/08/2019
-- Description:	Seleção de dados na tabela 
--              TB_Cliente para Combos
-- =============================================
ALTER PROCEDURE [dbo].[prcClienteComboSelect]
	@p_nidUsuario				NUMERIC(18,0)	= NULL

AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		DECLARE @p_CLIENTE INT = NULL
		SET @p_CLIENTE = (SELECT TOP 1 CD_CLIENTE FROM tbUsuarioCliente WHERE nidUsuario = @p_nidUsuario)
		IF (@p_CLIENTE > 0)
		BEGIN 
			SELECT
				TB_CLIENTE.CD_CLIENTE,
				TB_CLIENTE.NM_CLIENTE,
				TB_CLIENTE.EN_CIDADE,
				TB_CLIENTE.EN_ESTADO,
				TB_CLIENTE.DT_DESATIVACAO
			FROM	TB_CLIENTE
			LEFT JOIN dbo.TB_TECNICO_CLIENTE 
			ON TB_CLIENTE.CD_CLIENTE = @p_CLIENTE
			LEFT JOIN dbo.TB_TECNICO
			ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO
			WHERE 
			( TB_CLIENTE.CD_CLIENTE = @p_CLIENTE OR @p_CLIENTE IS NULL )
			AND ( TB_TECNICO_CLIENTE.CD_ORDEM < 2 OR TB_TECNICO_CLIENTE.CD_ORDEM IS NULL )
			AND ( TB_TECNICO.FL_ATIVO = 'S' or TB_TECNICO.FL_ATIVO IS NULL)
			ORDER BY
			TB_CLIENTE.NM_CLIENTE,
			TB_CLIENTE.CD_CLIENTE
		END 
		ELSE IF dbo.fncRestringirConsultaUsuario(ISNULL(@p_nidUsuario, 0)) = 1
		BEGIN 
			SELECT
				TB_CLIENTE.CD_CLIENTE,
				TB_CLIENTE.NM_CLIENTE,
				TB_CLIENTE.EN_CIDADE,
				TB_CLIENTE.EN_ESTADO,
				TB_CLIENTE.DT_DESATIVACAO
			FROM	TB_CLIENTE
			LEFT JOIN dbo.TB_TECNICO_CLIENTE 
			ON TB_CLIENTE.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE 
			LEFT JOIN dbo.TB_TECNICO
			ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO
			WHERE 
			( TB_TECNICO_CLIENTE.CD_ORDEM < 2 OR TB_TECNICO_CLIENTE.CD_ORDEM IS NULL )
			AND ( TB_TECNICO_CLIENTE.CD_CLIENTE IN (
				SELECT CD_CLIENTE FROM TB_TECNICO_CLIENTE WHERE TB_TECNICO_CLIENTE.CD_ORDEM < 2 AND CD_TECNICO IN (
					SELECT TB_TECNICO.CD_TECNICO FROM TB_TECNICO WHERE TB_TECNICO.ID_USUARIO IN (
						SELECT nidUsuario FROM fncRetornaUsuariosAcesso(@p_nidUsuario)) GROUP BY CD_TECNICO
							) GROUP BY CD_CLIENTE) OR @p_nidUsuario IS NULL)

			AND ( TB_TECNICO.FL_ATIVO = 'S' or TB_TECNICO.FL_ATIVO IS NULL)
			ORDER BY
				TB_CLIENTE.NM_CLIENTE,
				TB_CLIENTE.CD_CLIENTE

		END 
		ELSE 
		BEGIN
			-- Se não fizer parte, faz a consulta normal  
			SELECT
				TB_CLIENTE.CD_CLIENTE,
				TB_CLIENTE.NM_CLIENTE,
				TB_CLIENTE.EN_CIDADE,
				TB_CLIENTE.EN_ESTADO,
				TB_CLIENTE.DT_DESATIVACAO
			FROM	TB_CLIENTE
			LEFT JOIN dbo.TB_TECNICO_CLIENTE 
			ON TB_CLIENTE.CD_CLIENTE = @p_CLIENTE
			LEFT JOIN dbo.TB_TECNICO
			ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO
			WHERE 
			( TB_TECNICO_CLIENTE.CD_ORDEM < 2 OR TB_TECNICO_CLIENTE.CD_ORDEM IS NULL )
			AND ( TB_TECNICO.FL_ATIVO = 'S' or TB_TECNICO.FL_ATIVO IS NULL)
			ORDER BY
				TB_CLIENTE.NM_CLIENTE,
				TB_CLIENTE.CD_CLIENTE
		END  
	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH

END