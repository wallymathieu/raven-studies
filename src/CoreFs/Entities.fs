namespace SomeBasicRavenApp.Core.Entities
open System
open System.Collections.Generic
type Order = {mutable Id:string;mutable OrderDate:DateTime;mutable CustomerId:string;Products:List<string>;mutable Version:int }
and Customer= {mutable Id:string;mutable Firstname:string;mutable Lastname:string;mutable Version:int}
and Product= {mutable Id:string;mutable Cost:float;mutable Name:string;Products: List<string>;mutable Version:int}
module Product=
    let id (p:Product)=p.Id
module Customer=
    let id (p:Customer)=p.Id
module Order=
    let id (p:Order)=p.Id
