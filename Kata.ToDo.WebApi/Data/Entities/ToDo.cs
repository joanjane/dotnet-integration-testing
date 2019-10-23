using System;

namespace Kata.ToDo.WebApi.Data.Entities
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
    }
}