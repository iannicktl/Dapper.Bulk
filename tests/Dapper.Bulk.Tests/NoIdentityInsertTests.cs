using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dapper.Bulk.Tests
{
    public class NoIdentityInsertTests : SqlServerTestSuite
    {
        [Table("NoIdentityTests")]
        private class Node
        {
            public int ItemId { get; set; }
            
            public string Name { get; set; }
        }

        [Fact]
        public void InsertBulk()
        {
            var data = new List<Node>();
            for (int i = 0; i < 10; i++)
            {
                data.Add(new Node { ItemId = i, Name = Guid.NewGuid().ToString() });
            }
            
            using (var connection = this.GetConnection())
            {
                connection.Open();
                var inserted = connection.BulkInsert(data).ToList();
                for (int i = 0; i < inserted.Count; i++)
                {
                    IsValidInsert(inserted[i], data[i]);
                }
            }
        }
        
        [Fact]
        public void InsertSingle()
        {
            var item = new Node { ItemId = 1, Name = Guid.NewGuid().ToString() };
            using (var connection = this.GetConnection())
            {
                connection.Open();
                var inserted = connection.BulkInsert(new List<Node> { item }).First();
                IsValidInsert(inserted, item);
            }
        }

        [Fact]
        public async Task InsertSingleAsync()
        {
            var item = new Node { ItemId = 1, Name = Guid.NewGuid().ToString() };
            using (var connection = this.GetConnection())
            {
                connection.Open();
                var inserted = (await connection.BulkInsertAsync(new List<Node> { item })).First();
                IsValidInsert(inserted, item);
            }
        }

        [Fact]
        public void InsertSingleTransaction()
        {
            var item = new Node { ItemId = 1, Name = Guid.NewGuid().ToString() };
            using (var connection = this.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var inserted = connection.BulkInsert(new List<Node> { item }, transaction).First();
                    IsValidInsert(inserted, item);
                }
            }
        }

        private void IsValidInsert(Node inserted, Node toBeInserted)
        {
            inserted.ItemId.Should().Be(toBeInserted.ItemId);
            inserted.Name.Should().Be(toBeInserted.Name);
        }
    }
}