Crea un e-commerce in Spring Boot con le seguenti caratteristiche:

- Struttura base con `spring-boot-starter-web`, `spring-boot-starter-thymeleaf`, `spring-boot-starter-data-jpa` e `spring-boot-starter-security`.
- Entità `Product` con campi: `id`, `name`, `price`, `description`.
- Repository JPA per gestire i prodotti.
- Controller REST per CRUD (GET, POST, PUT, DELETE) con endpoint `/api/products`.
- Pagina web con Thymeleaf per visualizzare e gestire i prodotti (lista, aggiungi, modifica, elimina).
- Autenticazione basata su Spring Security con utente predefinito (`admin/password`).
- Configurazione `application.properties` per database H2 in memoria.
- Gestione errori con `@ControllerAdvice` e `@ExceptionHandler`.
- Utilizzo di `@Transactional` per le operazioni di persistenza.

Struttura pacchetti: `com.ecommerce.controller`, `com.ecommerce.model`, `com.ecommerce.repository`, `com.ecommerce.service`, `com.ecommerce.config`.