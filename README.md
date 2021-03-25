# Test Sysplan

O teste apresentado tem como objetivo avaliar os seguintes itens:

- SOLID
- TDD (Test-Driven Development)
- Clean Architecture
- Hexagonal Architecure (Portas e Protocolos)
- .NET Core 3.1;
- Web REST API;
- API Versioning;
- Swagger
- MSSQL Server
- Serilog para logging
- ELK - Elastic Stack (logstach, Elasticsearch e Kibana)
- GIT
- Docker
- AMQP (RabitMQ)

## Descrição

A solução apresentada corresponde a uma API e dois microsserviços.
Cada microsserviço apresentado consome uma fila específica.

A API possui os metodos de CRUD para cada controller correspondente 
a uma entidade. No caso encontrará apenas Client.

A arquitetura adotada para este teste foi a arquitetura limpa, ou clean architecture.

![image](https://user-images.githubusercontent.com/10169901/112555945-f25a6f80-8da7-11eb-8a5e-60e97b3a8d15.png)
[Figura1 -  Diagrama Arquitetura Limpa]

Como pode ser observado na imagem acima (Figura1), a arquitetura limpa nos leva diretamente ao
uso de boas práticas como DDD e SOLID, visto que abstraímos o problema proposto, olhando primeiramente
para o negócio, entender as regras de negócio, para compreender o problema; Delimitar o contexto
de negócio que estamos tratando, e a partir desse ponto, desenhar um mapa de contexto, a interação
entre as entidades encontradas. Nesse momento, e apenas, neste momeneto podemos definir qual a melhor
arquitetura que pode ser aplicada para o context map.

A não aplicação do DDD e SOLID, sendo SOLID toda a base de boas práticas para boas aplicações,
levará inevitavelmente ao fracasso de um projeto no momento que este precisar escalar.

