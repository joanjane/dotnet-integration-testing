using Kata.ToDo.WebApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kata.ToDo.WebApi.IntegrationTests
{
    [TestClass]
    public class ToDoTests
    {
        [TestMethod]
        public async Task GivenNoTodos_WhenListingTodos_ThenReturnEmpty()
        {
            // Arrange
            var factory = new CustomWebApplicationFactory();
            var httpClient = factory.CreateClient();

            // Act
            var response = await httpClient.GetAsync("/api/todo");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<IEnumerable<ToDoModel>>(content);
            Assert.AreEqual(0, values.Count());
        }

        [TestMethod]
        public async Task GivenOneToDo_WhenListingTodos_ThenReturnOne()
        {
            // Arrange
            Guid noteId = Guid.Empty;
            const string noteText = "Nota #1";
            var factory = new CustomWebApplicationFactory(async (dbContext) =>
            {
                var result = dbContext.ToDos.Add(new Data.Entities.ToDo
                {
                    Text = noteText
                });
                noteId = result.Entity.Id;
                await dbContext.SaveChangesAsync();
            });
            var httpClient = factory.CreateClient();

            // Act
            var response = await httpClient.GetAsync("/api/todo");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<IEnumerable<ToDoModel>>(content);
            Assert.AreEqual(1, values.Count());
            Assert.AreEqual(noteId, values.Single().Id);
            Assert.AreEqual(noteText, values.Single().Text);
        }

        [TestMethod]
        public async Task GivenNoToDos_WhenAddingTodo_ThenReturnInserted()
        {
            // Arrange
            var factory = new CustomWebApplicationFactory();
            var httpClient = factory.CreateClient();

            // Act
            const string noteText = "Nota #2";
            var newNoteRequest = new ToDoRequestModel
            {
                Text = noteText
            };
            //var requestContent = new StringContent(JsonConvert.SerializeObject(newNoteRequest), Encoding.UTF8, "application/json");
            //var response = await httpClient.PostAsync("/api/todo", requestContent);
            var response = await httpClient.PostAsJsonAsync("/api/todo", newNoteRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var newNote = JsonConvert.DeserializeObject<ToDoModel>(content);
            Assert.AreNotEqual(Guid.Empty, newNote.Id);
            Assert.AreEqual(noteText, newNote.Text);
        }

        [TestMethod]
        public async Task GivenExistingToDo_WhenUpdatingText_ThenTextIsChanged()
        {
            // Arrange
            Guid noteId = Guid.Empty;
            const string noteText = "Nota #3 actualizada";
            var factory = new CustomWebApplicationFactory(async (dbContext) =>
            {
                var result = dbContext.ToDos.Add(new Data.Entities.ToDo
                {
                    Text = "Nota #3 original"
                });
                noteId = result.Entity.Id;
                await dbContext.SaveChangesAsync();
            });
            var httpClient = factory.CreateClient();

            // Act
            var updateNoteRequest = new ToDoRequestModel
            {
                Text = noteText
            };
            var response = await httpClient.PutAsJsonAsync($"/api/todo/{noteId}", updateNoteRequest);

            // Assert
            response.EnsureSuccessStatusCode();

            var listResponse = await httpClient.GetAsync($"/api/todo");
            var values = JsonConvert.DeserializeObject<IEnumerable<ToDoModel>>(
                await listResponse.Content.ReadAsStringAsync());
            Assert.AreEqual(1, values.Count());
            Assert.AreEqual(noteId, values.Single().Id);
            Assert.AreEqual(noteText, values.Single().Text);
        }
    }
}
