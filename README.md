# ADSebraeTest

Teste de acesso autenticado ao domínio.

Simula o mesmo processo de autenticação utilizado pela aplicação e que tem apresentado erros intermitentes ao longo do dia nesse Estado.

O usuário acessa o domínio autenticado (precisa ser fornecido o usuário e senha de acesso ao AD) e em seguida tenta encontrar o usuário da aplicação no domínio.

Nesse momento é gerado uma exceção de usuário e senha inválidos, se referindo ao usuário de acesso ao domínio.

A senha do usuário da aplicação é irrelevante nessa etapa, o erro ocorre antes da sua autenticação.

# Release

A última release, com executável compilado, pode ser encontrado em: 

https://github.com/edgarrc/ADSebraeTest/releases

# Runtime

É necessário a existência do .Net Framework instalado.

Runtime: v4.0
Sku: .NETFramework,Version=v4.6.1
