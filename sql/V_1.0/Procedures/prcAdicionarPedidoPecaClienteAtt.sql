GO
/****** Object:  StoredProcedure [dbo].[prcAdicionarPedidoPecaClienteAtt]    Script Date: 02/09/2021 08:36:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Andre Farinelli
-- Create date: 06/11/2018
-- Description:	Gerar lote de aprovação de peças
-- de uma solicitação
-- =============================================
CREATE PROCEDURE [dbo].[prcAdicionarPedidoPecaClienteAtt]
	@p_ID_ESTOQUE								NUMERIC(9,0)	= NULL,
	@p_QT_PECA_ATUAL							DECIMAL(9,0)	= NULL,
	@p_QT_PECA_MIN								DECIMAL(9,0)	= NULL,
	@p_CD_PECA									VARCHAR (15)	= NULL,
	@p_DT_ULT_MIN								DATETIME		= Null
AS
BEGIN

	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--SET FMTONLY OFF;
	--SET XACT_ABORT ON;

	BEGIN

		--BEGIN TRANSACTION
		insert into tbEstoquePeca (CD_PECA, QT_PECA_ATUAL, QT_PECA_MIN, DT_ULT_MOVIM, ID_ESTOQUE)
			values
				(@p_CD_PECA, @p_QT_PECA_ATUAL, @p_QT_PECA_MIN, @p_DT_ULT_MIN, @p_ID_ESTOQUE)
            
		--Correção 8/4/19 erro na aprov avulsa e status aprov manual:
		
		--COMMIT TRANSACTION
	END

END