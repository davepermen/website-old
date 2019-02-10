let fixLinks = () => {
    document.querySelectorAll('a').forEach(element => {
        element.addEventListener('click', event => {
            event.preventDefault()
            let href = element.getAttribute('href')
            page(href)
        })
    })
}

let loadPage = async context => {
    if (context.init !== true) {
        let path = context.pathname
        let response = await (await fetch(path)).text()
        let body = {
            start: response.indexOf('<body>') + '<body>'.length,
            end: response.indexOf('</body>')
        }
        let content = response.substring(body.start, body.end)
        document.querySelector('body').innerHTML = content

        fixLinks()
    }
}

window.onload = () => {
    page('/', loadPage)
    page('/menu', loadPage)
    page()

    fixLinks()
}