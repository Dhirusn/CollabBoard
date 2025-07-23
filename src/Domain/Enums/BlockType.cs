using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabBoard.Domain.Enums;
public enum BlockType
{
    Text,      // rich-text runs
    Image,     // raster or SVG <image>
    Icon,      // SVG symbol
    Shape,     // rectangle, ellipse, connector
    Table      // grid of cells
}
