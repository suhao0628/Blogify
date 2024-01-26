﻿using System.ComponentModel.DataAnnotations;
using Blogify_API.Dtos.User;

namespace Blogify_API.Dtos.Community
{
    public class CommunityFullDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public bool IsClosed { get; set; } = false;

        [Required]
        public int SubscribersCount { get; set; } = 0;

        [Required]
        public List<UserDto> Administrators { get; set; }
    }
}
