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

![image](https://user-images.githubusercontent.com/10169901/112558272-22f0d800-8dad-11eb-9457-2cac1752da60.png)
 [Figura2 -  Diagrama Arquitetura Hexagonal]

Sobre o uso da arquitetura hexagonal (Figura2), a sua escolha determina que a solução proposta é de
projetos desacoplados. A comunicação entre eles devem ser por meio de portas e adaptadores.

No nosso exemplo não temos um projeto Aplicação, foi proposto apenas uma API e os microsserviços.
Entretando, caso quisessemos incluir uma UI/UX esta seria a nossa aplicação, e a mesma comunicariasse
com a camada API através de um protocolo bem definido, HTTP. Ou seja, se precisar dar manutenção
na camada Aplicação, não deve gerar retestagem na camada API e vice-versa.

A mesma regra se aplica aos serviços, no caso, os microsserviços não comunicam-se diretamente com a API
ou Aplicação, o protocolo usado nesse caso foi o AMQP, através de um Message Broker (RabbitMQ).
Como dito acima, se necessário manutenção em um dos microsserviços não pode afetar as outras camadas da aplicação.





