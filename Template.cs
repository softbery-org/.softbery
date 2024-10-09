// Version: 10.0.0.166
// Version: 10.0.0.44
// Version: 10.0.0.43
// Version: 10.0.0.42
// Version: 10.0.0.40
// Version: 10.0.0.38
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softbery
{
    internal class Template
    {
        public string Value =    "/* " +
                                               " * MIT License" +
                                               " * " +
                                               " * Copyright (c) {year} {company} by {author}" +
                                               " * " +
                                               " * Permission is hereby granted, free of charge, to any person obtaining a copy" +
                                               " * of this software and associated documentation files (the \"Software\"), to deal" +
                                               " * in the Software without restriction, including without limitation the rights" +
                                               " * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell" +
                                               " * copies of the Software, and to permit persons to whom the Software is" +
                                               " * furnished to do so, subject to the following conditions:" +
                                               " * " +
                                               " * The above copyright notice and this permission notice shall be included in all" +
                                               " * copies or substantial portions of the Software." +
                                               " * " +
                                               " * THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR" +
                                               " * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY," +
                                               " * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE" +
                                               " * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER" +
                                               " * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM," +
                                               " * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE" +
                                               " * SOFTWARE." +
                                               " *" +
                                               " * Project						: {project} " +
                                               " * " +
                                               " * File							: {file}" +
                                               " * Description				: {description}" +
                                               " *" +
                                               " *" +
                                               " * Author						: {author}" +
                                               " * Email							: {email}" +
                                               " * Website						: {website}" +
                                               " * Create	    				: {create}" +
                                               " * Last Modification Date: {modified}" +
                                               " */";
        public string MIT = 
               "/* \r\n * MIT License\r\n " +
                "* \r\n " +
                "* Copyright (c) {year} {company} by {author}\r\n " +
                "* \r\n " +
                "* Permission is hereby granted, free of charge, to any person obtaining a copy\r\n " +
                "* of this software and associated documentation files (the \"Software\"), to deal\r\n " +
                "* in the Software without restriction, including without limitation the rights\r\n " +
                "* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell\r\n " +
                "* copies of the Software, and to permit persons to whom the Software is\r\n " +
                "* furnished to do so, subject to the following conditions:\r\n * \r\n " +
                "* The above copyright notice and this permission notice shall be included in all\r\n " +
                "* copies or substantial portions of the Software.\r\n * \r\n " +
                "* THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\r\n " +
                "* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\r\n " +
                "* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\r\n " +
                "* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\r\n " +
                "* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\r\n " +
                "* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE\r\n " +
                "* SOFTWARE.\r\n " +
                "* \r\n " +
                "* Author\t\t\t\t\t\t: {author}\r\n " +
                "* Email\t\t\t\t\t\t\t: {email}\r\n " +
                "* Description\t\t\t\t\t: {description}\r\n " +
                "* Create\t\t\t\t\t\t: {create}\r\n " +
                "* Last Modification Date: {modified} \r\n " +
                "*/";
    }
}
