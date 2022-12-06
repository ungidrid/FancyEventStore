# Projects Purpose

## Actors
### Library which allows to implement Actor Model in .Net - *was NOT developed as a part of the master thesis*
* AntActor.Core

## Infrastructure
### Libraries, which *were developed as a part of the master thesis* to use Event Sourcing with different databases
* FancyEventStore.EventStore - base classes and interfaces for Event Store
* FancyEventStore.EfCoreStore - implementation of Event Store for SQL DB using Entity Framework Core
* FancyEventStore.DapperProductionStore - implementation of Event Store for SQL DB using Dapper ORM
* FancyEventStore.MongoDbStore - implementation of Event Store for Mongo DB using MongoDB Driver
* FancyEventStore.EventStoreDb - implementation of Event Store using Event Store DB (for comparison with other approaches)
* FancyEventStore.Common - shared code/helper methods

## Tests 
### Projects, which contain performance tests of the developed libraries. It *was developed as a part of the master thesis*
* FancyEventStore.DirectTests

## Example 
### Projects, which contain examples of how the developed libraries could be used in production. *Was developed as a part of the master thesis for the ease of explanation but is not a part of final work/product*
* FancyEventStore.Api
* FancyEventStore.ReadModel

## Common 
### Shared domain model, which is used for both *Test* and *Example* projects. *Was developed as a part of the master thesis for the ease of explanation but is not a part of final work/product*
* FancyEventStore.Domain
* FancyEventStore.Repositories
* FancyEventStore.Application




# Призначення проектів

## Модель акторів 
### Бібліотека, що дозволяє використовувати модель акторів у середовищі .Net. - *Не була розроблена у межах магістерської роботи, а лише використовується у ній*
* AntActor.Core

## Інфраструктура 
### Бібліотеки, що *були розроблені напряму для досягнення мети магістерської роботи* та дозволяють використовувати Event Sourcing у середовищі .Net з різними базами даних
* FancyEventStore.EventStore - базові класи та інтерфейси для сховища подій
* FancyEventStore.EfCoreStore - реалізація сховища подій для SQL баз даних з використанням Entity Framework Core
* FancyEventStore.DapperProductionStore - реалізація сховища подій для SQL баз даних з використанням Dapper ORM
* FancyEventStore.MongoDbStore - реалізація сховища подій для NoSQL бази даних Mongo DB з використанням MongoDB Driver
* FancyEventStore.EventStoreDb - реалізація сховища подій з використанням EventStoreDB (для порівняння з іншими підходами)
* FancyEventStore.Common - спільний код та допоміжні методи

## Tests 
### Проекти, що містять код для тестування швидкодії розроблених бібліотек. *Розроблені для досягнення цілей магістерської роботи*
* FancyEventStore.DirectTests

## Example 
### Проекти, що місять приклади того, як розроблені бібліотеки можуть використовуватись на реальних проектах. *Були розроблені як частина магістерської роботи, проте використовуються лише як приклад. Не використовуються для досягнення цілей роботи*
* FancyEventStore.Api
* FancyEventStore.ReadModel

## Common 
### Доменна модель, що використовується у проектах з секцій *Tests* та *Example*. *Були розроблені як частина магістерської роботи, проте використовуються лише як приклад. Не використовуються для досягнення цілей роботи*
* FancyEventStore.Domain
* FancyEventStore.Repositories
* FancyEventStore.Application
