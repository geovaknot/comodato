USE [COMODATOHOM]
GO
/****** Object:  StoredProcedure [dbo].[prcTecnicoClienteSelectQtdeTecnicos]    Script Date: 29/03/2023 11:58:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_Tecnico_Cliente
-- =============================================
ALTER PROCEDURE [dbo].[prcTecnicoClienteSelectQtdeTecnicos]
	@p_nidUsuario					NUMERIC(18,0)	= NULL,
	@p_CD_Cliente					INT				= NULL,	
	@p_CD_Tecnico					VARCHAR(06)		= NULL,
	@p_nvlQtdeTecnicos				INT				= NULL
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
		
		CREATE TABLE #tbClienteQuantidade
		(
			CD_CLIENTE				NUMERIC(6,0)	NULL,
			NM_CLIENTE				VARCHAR(50)		NULL,
			EN_CIDADE				VARCHAR(50)		NULL,
			EN_ESTADO				VARCHAR(03)		NULL,
			CD_REGIAO				VARCHAR(02)		NULL,
			DS_REGIAO				VARCHAR(30)		NULL,
			DT_DESATIVACAO			DATETIME		NULL,
			nvlQtdeTecnicos			INT				NULL,
			CD_TECNICO_PRINCIPAL	VARCHAR(06)		NULL,
			NM_TECNICO_PRINCIPAL	VARCHAR(50)		NULL,
			QT_PERIODO				INT				NULL,
			nvlCargaTecnica			INT				NULL 
		)

		INSERT INTO #tbClienteQuantidade
	        ( CD_CLIENTE,
	          NM_CLIENTE,
	          EN_CIDADE,
	          EN_ESTADO,
			  CD_REGIAO,
			  DS_REGIAO,
			  DT_DESATIVACAO,
	          nvlQtdeTecnicos,
	          CD_TECNICO_PRINCIPAL,
	          NM_TECNICO_PRINCIPAL,
	          QT_PERIODO,
	          nvlCargaTecnica )
		SELECT 
			dbo.TB_CLIENTE.CD_CLIENTE,
			dbo.TB_CLIENTE.NM_CLIENTE,
			dbo.TB_CLIENTE.EN_CIDADE,
			dbo.TB_CLIENTE.EN_ESTADO,
			dbo.V_REGIAO.CD_REGIAO,
			dbo.V_REGIAO.DS_REGIAO,
			dbo.TB_CLIENTE.DT_DESATIVACAO,
			(SELECT COUNT(TC.CD_TECNICO) FROM dbo.TB_TECNICO as T
				LEFT JOIN dbo.TB_TECNICO_CLIENTE as TC ON TC.CD_TECNICO = T.CD_TECNICO
				WHERE (TC.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE AND T.FL_ATIVO = 'S')) as nvlQtdeTecnicos,
			(SELECT TOP 1 T.CD_TECNICO 
				FROM dbo.TB_TECNICO AS T
				INNER JOIN dbo.TB_TECNICO_CLIENTE AS TC
					ON T.CD_TECNICO = TC.CD_TECNICO
				WHERE TC.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
				AND TC.CD_ORDEM = 1 AND T.FL_ATIVO = 'S'
			) AS CD_TECNICO_PRINCIPAL,
			(SELECT TOP 1 T.NM_TECNICO 
				FROM dbo.TB_TECNICO AS T
				INNER JOIN dbo.TB_TECNICO_CLIENTE AS TC
					ON T.CD_TECNICO = TC.CD_TECNICO
				WHERE TC.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
				AND TC.CD_ORDEM = 1 AND T.FL_ATIVO = 'S'
			) AS NM_TECNICO_PRINCIPAL,
			ISNULL(dbo.TB_CLIENTE.QT_PERIODO,0) AS QT_PERIODO,
			(SELECT SUM(ISNULL(C.QT_PERIODO,0))
				FROM dbo.TB_CLIENTE AS C
				INNER JOIN dbo.TB_TECNICO_CLIENTE AS TC
					ON C.CD_CLIENTE = TC.CD_CLIENTE
				WHERE TC.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
				AND TC.CD_ORDEM = 1 
			) AS nvlCargaTecnica
		FROM 
			dbo.TB_CLIENTE
			LEFT OUTER JOIN dbo.TB_TECNICO_CLIENTE
			ON dbo.TB_CLIENTE.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
			LEFT OUTER JOIN dbo.V_REGIAO
			ON dbo.TB_CLIENTE.CD_REGIAO = dbo.V_REGIAO.CD_REGIAO
		WHERE	
				(dbo.TB_CLIENTE.CD_CLIENTE		= @p_CD_Cliente		OR @p_CD_Cliente	IS NULL )
			AND	dbo.TB_CLIENTE.DT_DESATIVACAO	IS NULL 
		GROUP BY 
			dbo.TB_CLIENTE.CD_CLIENTE,
			dbo.TB_CLIENTE.NM_CLIENTE,
			dbo.TB_CLIENTE.EN_CIDADE,
			dbo.TB_CLIENTE.EN_ESTADO,
			dbo.V_REGIAO.CD_REGIAO,
			dbo.V_REGIAO.DS_REGIAO,
			dbo.TB_CLIENTE.DT_DESATIVACAO,
			dbo.TB_CLIENTE.QT_PERIODO

		IF dbo.fncRestringirConsultaTecnicoUsuario(ISNULL(@p_nidUsuario, 0)) = 1 
		BEGIN
			--Busca somente os clientes associados ao usuário vinculado a um determinado técnico
			SELECT * FROM #tbClienteQuantidade
			WHERE	( CD_TECNICO_PRINCIPAL	= @p_CD_Tecnico			OR @p_CD_Tecnico		IS NULL )
				AND	( nvlQtdeTecnicos		= @p_nvlQtdeTecnicos	OR @p_nvlQtdeTecnicos	IS NULL )
				-- complementa a busca com os clientes somente vinculados ao técnico do usuário informado
				AND ( CD_CLIENTE IN ( SELECT DISTINCT 
												dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
											FROM dbo.TB_TECNICO 
											INNER JOIN dbo.TB_TECNICO_CLIENTE 
											ON dbo.TB_TECNICO.CD_TECNICO = dbo.TB_TECNICO_CLIENTE.CD_TECNICO
											WHERE ID_USUARIO = @p_nidUsuario
											)
			)
			ORDER BY
			NM_CLIENTE,
			CD_CLIENTE,
			NM_TECNICO_PRINCIPAL
		END
		ELSE
		BEGIN  
			-- Se não fizer parte, faz a consulta normal
			SELECT * FROM #tbClienteQuantidade
			WHERE	( CD_TECNICO_PRINCIPAL	= @p_CD_Tecnico			OR @p_CD_Tecnico		IS NULL )
				AND	( nvlQtdeTecnicos		= @p_nvlQtdeTecnicos	OR @p_nvlQtdeTecnicos	IS NULL )
			ORDER BY 
				NM_CLIENTE,
				CD_CLIENTE,
				NM_TECNICO_PRINCIPAL
		END

		If(OBJECT_ID('tempdb..#tbClienteQuantidade') Is Not Null)
		BEGIN
			DROP TABLE #tbClienteQuantidade
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


