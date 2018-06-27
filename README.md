# CommonMark.Tables

A custom formatter to extend [CommonMark.NET](https://github.com/Knagis/CommonMark.NET) to render Markdown tables.

## Example

At the moment, this formatter only supports pipe-delimited columns/data and a skeletal table frame. Therefore,

```
Column 1 | Column 2
---|---
1 | 0
```

will yield

Column 1 | Column 2
---|---
1 | 0


## Roadmap

* State-based lookahead to detect table validity
* Support for a wider variety of table markup
* Nested Markdown in table headers and cells
* Robust pipe detection and rejection in cells

## License
[MIT](https://github.com/LoraxCompliance/CommonMark.Tables/blob/master/LICENSE)