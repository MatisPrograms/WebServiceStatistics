const websites = [];

function createItemList() {
    if (websites.length === 0) return;

    const table = document.getElementById('searchTable').querySelector('tbody');
    table.innerHTML = '';

    for (const website of websites) {
        const element = document.createElement('tr');
        const websiteName = website.Url
            .replace(/^https?:\/\//, '') // Remove the protocol (http or https)
            .split('/') // Split the URL by slashes
            .shift() // Get the first element (the domain name)
            .split('.') // Split the domain name by dots
            .reverse() // Reverse the array
            .slice(1) // Remove the last element (the TLD, e.g. "com")
            .reverse() // Reverse the array again to restore the original order
            .join('.'); // Join the array back into a string

        element.innerHTML = `
        <td class="sm:p-3 py-2 px-1 border-b border-gray-200 dark:border-gray-800">
            <div class="flex items-center">
                <img alt="PayPal Logo"
                     class="w-7 h-7 p-1.5 mr-2.5 rounded-lg border border-gray-200 dark:border-gray-800"
                     src="https://www.google.com/s2/favicons?domain=${website.Url}">
                ${websiteName.charAt(0).toUpperCase() + websiteName.slice(1)}
            </div>
        </td>
        <td class="sm:p-3 py-2 px-1 border-b border-gray-200 dark:border-gray-800">
            ${website.Url}
        </td>
        <td class="sm:p-3 py-2 px-1 border-b border-gray-200 dark:border-gray-800">
             ${website.Server ? website.Server : "N/A"}
        </td>
        <td class="sm:p-3 py-2 px-1 border-b border-gray-200 dark:border-gray-800">
             ${website.AverageAge ? typeof website.AverageAge === "number" ? website.AverageAge.toFixed(2) : "N/A" : "N/A"}
        </td>
        <td class="sm:p-3 py-2 px-1 border-b border-gray-200 dark:border-gray-800">
             ${website.DeviationAge ? typeof website.DeviationAge === "number" ? website.DeviationAge.toFixed(2) : "N/A" : "N/A"}
        </td>
        <td class="sm:p-3 py-2 px-1 border-b border-gray-200 dark:border-gray-800">
            <div class="flex items-center">
                <div class="sm:flex hidden flex-col">
                    ${website.AverageCookie ? typeof website.AverageCookie === "number" ? website.AverageCookie.toFixed(2) : "N/A" : "N/A"} 
                </div>
                <button class="w-8 h-8 inline-flex items-center justify-center text-gray-400 ml-auto">
                    <svg class="w-5" fill="none" stroke="currentColor" stroke-linecap="round"
                         stroke-linejoin="round" stroke-width="2" viewBox="0 0 24 24">
                        <circle cx="12" cy="12" r="1"></circle>
                        <circle cx="19" cy="12" r="1"></circle>
                        <circle cx="5" cy="12" r="1"></circle>
                    </svg>
                </button>
            </div>
        </td>
        `;
        table.append(element);
    }
}

function loadServerTypes(param) {
    fetch('/api/server' + (param ? "?url=" + param : ""), {
        method: 'GET',
    }).then(response => response.json()).then(data => {
        data.forEach(item => {
            if (!websites.map(e => e.Url).includes(item.Url)) websites.unshift(item);
        });

        websites.forEach(item => {
            const website = data.find(e => e.Url === item.Url);
            if (!website) return;

            const server = website.Server;
            item.Server = server ? server : item.Server ? item.Server : "N/A";
        });

        createItemList();

        // Group by server type and count
        const serverTypeCounts = websites.reduce((acc, serverType) => {
            const server = serverType.Server;
            acc[server] = acc[server] ? acc[server] + 1 : 1;
            return acc;
        }, {});

        generateChart(document.getElementById('server-type-chart'), {
            type: 'doughnut',
            data: {
                labels: Object.keys(serverTypeCounts),
                datasets: [{
                    label: 'Calculated on ' + websites.length + ' websites',
                    data: Object.values(serverTypeCounts),
                    fill: false,
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Website Server Types'
                    }
                }
            }
        });
    }).catch(error => console.log(error));
}

function loadAges(param) {
    fetch('/api/age' + (param ? "?url=" + param : ""), {
        method: 'GET',
    }).then(response => response.json()).then(data => {
        data.forEach(item => {
            if (!websites.map(e => e.Url).includes(item.Url)) websites.unshift(item);
        });

        websites.forEach(item => {
            const website = data.find(e => e.Url === item.Url);
            if (!website) return;

            const count = website.CountAge;
            const average = website.AverageAge;
            const deviation = website.DeviationAge;

            item.CountAge = count ? count : item.CountAge ? item.CountAge : "N/A";
            item.AverageAge = average ? average / 3600 : item.AverageAge ? item.AverageAge : "N/A";
            item.DeviationAge = deviation ? deviation / 3600 : item.DeviationAge ? item.DeviationAge : "N/A";
        });

        createItemList();

        const filteredWebsites = websites.filter(e => e.AverageAge !== "N/A" || e.DeviationAge !== "N/A" || e.CountAge !== "N/A");

        generateChart(document.getElementById('age-chart'), {
            type: 'bar',
            data: {
                labels: filteredWebsites.map(e => e.Url.replace("https://", "").replace("http://", "")),
                datasets: [
                    {
                        label: 'Number of Sub Urls tested',
                        data: filteredWebsites.map(e => e.CountAge === "N/A" ? 0 : e.CountAge),
                        fill: false,
                    },
                    {
                        label: 'μ Age (Average in hours)',
                        data: filteredWebsites.map(e => e.AverageAge === "N/A" ? 0 : e.AverageAge),
                        fill: false,
                    },
                    {
                        label: 'σ Age (Deviation in hours)',
                        data: filteredWebsites.map(e => e.DeviationAge === "N/A" ? 0 : e.DeviationAge),
                        fill: false,
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Website\'s Sub Urls Last Modified Age'
                    }
                }
            }
        });
    }).catch(error => console.log(error));
}

function loadCookies(param) {
    fetch('/api/cookie' + (param ? "?url=" + param : ""), {
        method: 'GET',
    }).then(response => response.json()).then(data => {
        data.forEach(item => {
            if (!websites.map(e => e.Url).includes(item.Url)) websites.unshift(item);
        });

        websites.forEach(item => {
            const website = data.find(e => e.Url === item.Url);
            if (!website) return;

            const count = website.CountCookie;
            const average = website.AverageCookie;
            const deviation = website.DeviationCookie;

            item.CountCookie = count ? count : item.CountCookie ? item.CountCookie : "N/A";
            item.AverageCookie = average ? average : item.AverageCookie ? item.AverageCookie : "N/A";
            item.DeviationCookie = deviation ? deviation : item.DeviationCookie ? item.DeviationCookie : "N/A";
        });

        createItemList();

        const filteredWebsites = websites.filter(e => e.AverageCookie !== "N/A" || e.DeviationCookie !== "N/A" || e.CountCookie !== "N/A");

        generateChart(document.getElementById('cookie-chart'), {
            type: 'bar',
            data: {
                labels: filteredWebsites.map(e => e.Url.replace("https://", "").replace("http://", "")),
                datasets: [
                    {
                        label: 'Number of Sub Urls tested',
                        data: filteredWebsites.map(e => e.CountCookie === "N/A" ? 0 : e.CountCookie),
                        fill: false,
                    },
                    {
                        label: 'μ Cookies (Average)',
                        data: filteredWebsites.map(e => e.AverageCookie === "N/A" ? 0 : e.AverageCookie),
                        fill: false,
                    },
                    {
                        label: 'σ Cookies (Deviation)',
                        data: filteredWebsites.map(e => e.DeviationCookie === "N/A" ? 0 : e.DeviationCookie),
                        fill: false,
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Website\'s Cookie Count per page'
                    }
                }
            }
        });
    }).catch(error => console.log(error));
}

function generateChart(element, config) {
    Chart.getChart(element)?.destroy();
    new Chart(element.getContext('2d'), config);
}

addEventListener('load', () => {

    loadServerTypes();
    loadAges();
    loadCookies();

    const menuButtons = document.querySelectorAll('.flex.mx-auto.flex-grow.mt-4.flex-col.text-gray-400.space-y-4 button');
    menuButtons.forEach(button => {
        button.addEventListener('click', () => {
            button.classList.add('dark:bg-gray-700', 'dark:text-white', 'bg-blue-100', 'text-blue-500');
            button.classList.remove('dark:text-gray-500');
            document.getElementById(button.id.replace('Button', '')).classList.remove('hidden');

            Array.from(menuButtons).filter(btn => btn !== button).forEach(btn => {
                btn.classList.add('dark:text-gray-500');
                btn.classList.remove('dark:bg-gray-700', 'dark:text-white', 'bg-blue-100', 'text-blue-500');
                document.getElementById(btn.id.replace('Button', '')).classList.add('hidden');
            });
        })
    });

    // Do action when enter was pressed
    const table = document.getElementById('searchTable').querySelector('tbody');
    const searchBox = document.getElementById('SearchInput');
    searchBox.addEventListener('keyup', (event) => {
        for (const row of table.querySelectorAll('tr')) {
            const websiteName = row.querySelector('td').textContent.trim().toLowerCase();
            const websiteUrl = row.querySelector('td:nth-child(2)').textContent.trim().toLowerCase();
            const serverType = row.querySelector('td:nth-child(3)').textContent.trim().toLowerCase();

            if (websiteName.startsWith(searchBox.value.toLowerCase())
                || websiteUrl.startsWith(searchBox.value.toLowerCase())
                || websiteUrl.startsWith("https://" + searchBox.value.toLowerCase())
                || serverType.startsWith(searchBox.value.toLowerCase())) row.classList.remove("hidden");
            else row.classList.add("hidden");
        }

        if (event.key === 'Enter' && searchBox.value.includes(".")) {
            loadServerTypes(searchBox.value);
            loadAges(searchBox.value);
            loadCookies(searchBox.value);
            searchBox.value = "";
        }
    });
})