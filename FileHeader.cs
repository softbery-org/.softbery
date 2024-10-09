// Version: 10.0.0.170
// Version: 10.0.0.48
// Version: 10.0.0.47
// Version: 10.0.0.46
// Version: 10.0.0.44
// Version: 10.0.0.42
/* 
 * MIT License
 * 
 * Copyright (c) 2023 Softbery by Paweł Tobis
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * Author						: Paweł Tobis
 * Email							: softbery@gmail.com
 * Description					:
 * Create						: 2023-02-24 04:31:42
 * Last Modification Date: 2024-01-30 19:58:04
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softbery
{

    internal class FileHeader
    {
        // Private variables
        private int _year = DateTime.Now.Date.Year;
        private string? _author = "Author";
        private string? _email = "author@example.com";
        private string? _website = "https://example.com/";
        private string? _project = "Project";
        private string? _description = "A example description for example file header.";
        private string? _template = new Template().Value;
        private bool? _hash = true;
        private bool? _file = true;
        private DateTime _create = DateTime.Now;
        private DateTime _modified = DateTime.Now;

        /// <summary>
        /// Author current code
        /// </summary>
        public string? Author { get => _author; set => _author = value; }
        /// <summary>
        /// Author email address
        /// </summary>
        public string? Email { get => _email; set => _email = value; }
        /// <summary>
        /// Project website
        /// </summary>
        public string? Website { get => _website; set => _website = value; }
        /// <summary>
        /// Project name
        /// </summary>
        public string? Project { get => _project; set => _project = value; }
        /// <summary>
        /// A current file hash
        /// </summary>
        public bool? Hash { get => _hash; set => _hash = value; }
        /// <summary>
        /// Project description
        /// </summary>
        public string? Description { get => _description; set => _description = value; }
        /// <summary>
        /// A file name
        /// </summary>
        public bool? FileName { get => _file; set => _file = value; }
        /// <summary>
        /// Template to file header
        /// </summary>
        public string? Template { get => _template; set => _template = value; }
        /// <summary>
        /// Create date
        /// </summary>
        /// <example>
        /// 1999-01-01 15:00:00
        /// </example>
        public DateTime Create { get => _create; set => _create = value; }
        /// <summary>
        /// Last modification
        /// </summary>
        /// <example>
        /// 1999-01-01 15:00:00
        /// </example>
        public DateTime Modified { get => _modified; set => _modified = value; }
        /// <summary>
        /// Current year
        /// </summary>
        public int Year { get => _year; set => _year = value; }

        private string _copyrightPattern = @"^.*\*.*Copyright \([Cc]\).*";
        private string _authorPattern = @"(^.*\*.*Author.*): (.*)";
        private string _emailPattern = @"(^.*\*.*Email.*): (.*)";
        private string _descriptionPattern = @"(^.*\*.*Description.*): (.*)";
        private string _createPattern = @"(^.*\*.*Create.*): (.*)";
        private string _modifiedPattern = @"(^.*\*.*Author.*): (.*)";

        /// <summary>
        /// Create a file header
        /// </summary>
        public FileHeader()
        {

        }


    }
}
