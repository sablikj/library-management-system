﻿using LibraryManagement.Models.Entity;
using MongoDB.Driver;

namespace LibraryManagement.Models.ViewModels
{
    public class IndexViewModel
    {
        public IList<CarouselItem> CarouselItem { get; set; }

        public IList<Book> Books { get; set; }
    }
}
