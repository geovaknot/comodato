GO
/****** Object:  StoredProcedure [dbo].[prcPlanoZeroControle]    Script Date: 21/02/2024 09:16:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Use COMODATODEV
-- exec prcPlanoZeroGerarPedidos 15434
ALTER Procedure [dbo].[prcPlanoZeroControle]
	@ID_USUARIO int
AS
BEGIN
	
	Begin 
		
	--  --Cria o Controle Plano Zero
    INSERT INTO tbControlePlanoZero (dtHoraCriacao, idUsuarioCriacao, dtHoraCancelamento, idUsuarioCancelamento, statusPlanoZero, mensagem)
    VALUES (GETDATE(), @ID_USUARIO, NULL, NULL, 'S', 'Processamento Plano Zero Iniciado')
	
	update tbParametro set cvlParametro = 'true' where ccdParametro = 'Plano_Zero_em_Processamento'
	END 

	
END
