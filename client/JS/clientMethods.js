// Function to check if the environment is development
function isDevEnv() {
    return location.host.includes('localhost') || location.host.includes('127.0.0.1');
}

// Declare backendBaseUrl
let backendBaseUrl;
if (isDevEnv()) {
    const backendPort = 54200; // Change manually if needed
    backendBaseUrl = `https://localhost:${backendPort}/api`; // Development (localhost backend)
} else {
    backendBaseUrl = `https://proj.ruppin.ac.il/cgroup1/test2/tar1/api`; // Production (deployed backend)
}

// Movie URLs
const postUrl = `${backendBaseUrl}/Movie`;
const getUrl = `${backendBaseUrl}/Movie`;
const searchUrl = `${backendBaseUrl}/Movie/search`;

const userApiBaseUrl = `${backendBaseUrl}/User`;
const loginUrl = `${userApiBaseUrl}/login`;
const registerUrl = `${userApiBaseUrl}/register`;

// Declare indexUrl
let indexUrl;
if (isDevEnv()) {
    const frontendPort = location.port;
    // More robust path generation
    const path = window.location.pathname.substring(0, window.location.pathname.lastIndexOf('/'));
    indexUrl = `${window.location.protocol}//${window.location.hostname}:${frontendPort}${path}/index.html`;
} else {
    indexUrl = `https://proj.ruppin.ac.il/cgroup1/test2/tar3/pages/index.html`; // Production (deployed frontend)
}

// Load movies functionality
let isLoaded = false;
function init() {
    checkAuthorization(); // Ensure the user is authorized to access the page
    loadNavbar();         // Load the navbar on page load
    showGreeting(); // Show Hello messeag with the user name


    // Hide the error message initially
    $('#registerError').addClass('hidden');
    $('#registerSuccess').addClass('hidden');

    // Initialize event listeners for login and registration
    $('#loginForm').on('submit', handleLogin);
    $('#registerForm').on('submit', handleRegister);

    // Setup search and rent modal events for all pages
    setupSearchFunctionality();
    setupRentModalEvents();

    // Page-specific logic
    const path = window.location.pathname;
    if (path.endsWith('/pages/MyMovies.html')) {
        loadMyRentedMovies();
        setupTransferModalEvents();
    } else if (path.endsWith('/pages/index.html') || path === '/') {
        getAllMoviesListFromServer();
    } else if (path.endsWith('/pages/addMovie.html')) {
        initAddMovieForm();
    } else if (path.endsWith('/pages/editProfile.html')) {
        initEditProfileForm();
    } else if (path.endsWith('/pages/admin.html')) {
        initAdminPage();
    }
}

function ajaxCall(method, api, data, successCB, errorCB) {
    $.ajax({
        type: method,
        url: api,
        data: data,
        cache: false,
        contentType: "application/json",
        dataType: "json",
        success: successCB,
        error: errorCB
    });
}

//------------------------------------------------------- Create and Render Movies Methods -----------------------------------------------
function createMovie(movieData) {
    const requiredFields = [
        "primaryTitle",
    ];

    for (let field of requiredFields) {
        if (movieData[field] === undefined || movieData[field] === null) {
            console.log(movieData);
            console.error(`Missing required field: ${field}`);
            return; // Stop early if any field is missing
        }
    }
    //console.log(movieData.releaseDate);
    return {
        Id: movieData.id, // Use the `id` from the database
        Url: movieData.url,
        PrimaryTitle: movieData.primaryTitle,
        Description: movieData.description,
        PrimaryImage: movieData.primaryImage,
        Year: movieData.startYear,
        ReleaseDate: movieData.releaseDate,
        Language: movieData.language,
        Budget: movieData.budget !== null ? movieData.budget : 0, // Replace null with 0
        GrossWorldwide: movieData.grossWorldwide !== null ? movieData.grossWorldwide : 0, // Replace null with 0
        Genres: movieData.genres.join(),
        IsAdult: movieData.isAdult,
        RuntimeMinutes: movieData.runtimeMinutes,
        AverageRating: movieData.averageRating,
        NumVotes: movieData.numVotes,
    };

}

function renderMovie(filteredMovieData, btnType) {
    // Use RentalId for rented movies, Id for general movies
    const movieDivId = (btnType === "deleteFromList" && filteredMovieData.RentalId)
        ? filteredMovieData.RentalId
        : filteredMovieData.Id;

    const movieDiv = $(`<div class="Movie" id="${movieDivId}"></div>`);
    $("#loadedMovies").append(movieDiv);

    const {
        PrimaryTitle,
        Description = "No description available", // Default value for missing description
        PrimaryImage = "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' enable-background='new 0 0 24 24' height='24' viewBox='0 0 24 24' width='24'%3E%3Cg%3E%3Crect fill='none' height='24' width='24'/%3E%3Cpath d='M21.9,21.9l-8.49-8.49l0,0L3.59,3.59l0,0L2.1,2.1L0.69,3.51L3,5.83V19c0,1.1,0.9,2,2,2h13.17l2.31,2.31L21.9,21.9z M5,18 l3.5-4.5l2.5,3.01L12.17,15l3,3H5z M21,18.17L5.83,3H19c1.1,0,2,0.9,2,2V18.17z'/%3E%3C/g%3E%3C/svg%3E",
        Year = "Unknown", // Default value for missing year
        ReleaseDate = "Unknown", // Default value for missing release date
        RuntimeMinutes = "N/A", // Default value for missing runtime
        AverageRating = "No rating yet", // Default value for missing rating
        Genres = "Unknown", // Default value for missing genres
        Budget = "N/A", // Default value for missing budget
        GrossWorldwide = "N/A", // Default value for missing box office
        NumVotes = "N/A", // Default value for missing votes
        Language = "English" // <-- Default to English if missing

    } = filteredMovieData;

    // Format the release date to European format (DD/MM/YYYY)
    let formattedReleaseDate = "Unknown";
    if (ReleaseDate !== "Unknown") {
        if (ReleaseDate.includes("T")) {
            // Handle format: YYYY-MM-DDTHH:mm:ss
            const [year, month, day] = ReleaseDate.split("T")[0].split("-");
            formattedReleaseDate = `${day}/${month}/${year}`;
        } else if (ReleaseDate.includes("-")) {
            // Handle format: YYYY-MM-DD
            const [year, month, day] = ReleaseDate.split("-");
            formattedReleaseDate = `${day}/${month}/${year}`;
        }
    }

    const upperDiv = $(`<div class="upperDiv"></div>`);
    const lowerDiv = $(`<div class="lowerDiv"></div>`);
    movieDiv.append(upperDiv, lowerDiv);

    if (btnType === "addToCart") {
        const releaseDate = new Date(filteredMovieData.ReleaseDate);
        const today = new Date();
        today.setHours(0, 0, 0, 0); // Normalize to compare dates only

        if (releaseDate > today) {
            const comingSoonBTN = $(`<button class="addToCartBTN" disabled>Coming Soon</button>`);
            upperDiv.append(comingSoonBTN);
        } else {
            const addToCartBTN = $(`<button class="addToCartBTN">Rent me</button>`);
            addToCartBTN.click(() => {
                const loggedInUser = localStorage.getItem('loggedInUser');
                if (!loggedInUser) {
                    alert("You must be logged in to rent.");
                    window.location.href = 'login.html';
                } else {
                    openRentModal(filteredMovieData);
                }
            });
            upperDiv.append(addToCartBTN);
        }
    }

    let transferBtn = null; // append tensferBtn only if exists

    if (btnType === "deleteFromList") {
        const deleteFromList = $(`<button class="deleteFromList">Remove</button>`);
        deleteFromList.click(() => {
            console.log(`Button has been pressed, parent is: ${filteredMovieData.Id}`);
            // Confirm before deleting
            if (confirm(`Are you sure you want to remove "${PrimaryTitle}" from your list?`)) {
                deleteFromServer(filteredMovieData.Id, movieDiv);
            }
        });

        upperDiv.append(deleteFromList);

        // Add Transfer button
        const transferBtn = $(`<button class="transferMovieBTN">Transfer</button>`);
        transferBtn.click(() => {
            openTransferModal({ ...filteredMovieData, rentalId: filteredMovieData.RentalId || filteredMovieData.Id });
        });
        // Add at the bottom of lowerDiv
        lowerDiv.append(transferBtn);
    }

    const ratingDiv = $(`<div class="rating">${AverageRating}</div>`);
    const altImgText = `${PrimaryTitle} movie poster`;
    const imgDiv = $(`<img src="${PrimaryImage}" alt="${altImgText}">`);

    upperDiv.append(ratingDiv, imgDiv);

    // Updated sections with explicit class names
    const titleDiv = $(`<div class="movie-title">${PrimaryTitle}</div>`); // Title
    const yearDiv = $(`<div class="movie-year">${Year}</div>`); // Year
    const releaseDateDiv = $(`<div class="movie-release-date">Release Date: ${formattedReleaseDate}</div>`); // Release Date
    const runtimeDiv = $(`<div class="movie-runtime">${RuntimeMinutes} min</div>`); // Runtime
    const genresDiv = $(`<div class="movie-genres">Genres: ${Genres}</div>`); // Genres
    const budgetDiv = $(`<div class="movie-budget">Budget: $${Budget.toLocaleString()}</div>`); // Budget
    const boxOfficeDiv = $(`<div class="movie-box-office">Box Office: $${GrossWorldwide.toLocaleString()}</div>`); // Box Office
    const votesDiv = $(`<div class="movie-votes">Votes: ${NumVotes.toLocaleString()}</div>`); // Votes
    const descriptionDiv = $(`<div class="movie-description">${Description}</div>`); // Description
    const rentalCountDiv = $(`<div class="movie-rental-count">Rented: ${filteredMovieData.RentalCount || 0} times</div>`);
    const languageDiv = $(`<div class="movie-language">Language: ${Language}</div>`);


    // Rental info
    let rentalInfoDiv = "";
    if (filteredMovieData.RentStart && filteredMovieData.RentEnd && filteredMovieData.TotalPrice !== undefined) {
        // Format dates
        const formatDate = (dateStr) => {
            if (!dateStr) return "N/A";
            const d = new Date(dateStr);
            if (isNaN(d)) return dateStr;
            return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()}`;
        };
        rentalInfoDiv = $(`
            <div class="rental-info">
                <strong>Rented:</strong> ${formatDate(filteredMovieData.RentStart)} - ${formatDate(filteredMovieData.RentEnd)}<br>
                <strong>Price Paid:</strong> $${filteredMovieData.TotalPrice}
            </div>
        `);
    }

    lowerDiv.append(
        titleDiv,
        yearDiv,
        runtimeDiv,
        genresDiv,
        releaseDateDiv,
        budgetDiv,
        boxOfficeDiv,
        votesDiv,
        languageDiv,
        descriptionDiv,
        rentalInfoDiv,
        rentalCountDiv,
    );

}
// Fetches all movies from the server and renders them on the page.
function getAllMoviesListFromServer(page = 1, pageSize = 20) {
    ajaxCall(
        "GET",
        `${getUrl}?page=${page}&pageSize=${pageSize}`,
        "",
        insertMovieSCB,
        insertMovieECB
    );
}

 //Success Callback
function insertMovieSCB(response) {
    renderMyList(response.movies, "addToCart");
    renderPagination(response.totalCount, page =  1, pageSize = 20);
}

// Error callback
function insertMovieECB(err) {
    console.error(`Error: ${err}`);
}

//-------------------------------------------------------- Main Page + My Movies Methods -------------------------------------------------------------
// function responsible for rendering the list of movies in the user's collection.
function renderMyList(moviesFromServer, btnType = "addToCart") {
    // Clear existing movies first to avoid duplicates
    $("#loadedMovies").empty();
    if (!moviesFromServer || moviesFromServer.length === 0) {
        $("#loadedMovies").append("<p>No movies in your collection yet.</p>");
        return;
    }

    // Render each movie with the delete button 
    moviesFromServer.forEach(movie => {
        console.log("Rendering Movie:", movie); // Log each movie being rendered
        console.log("Raw movie from server:", movie);
        console.log("RentalId candidates:", movie.rentalId, movie.RentalId, movie.Id);

        const normalizedMovie = {
            Id: movie.id,
            PrimaryTitle: movie.primaryTitle || movie.PrimaryTitle,
            Description: movie.description || movie.Description,
            PrimaryImage: movie.primaryImage || movie.PrimaryImage,
            Year: movie.year,
            ReleaseDate: movie.releaseDate, // Ensure releaseDate is passed
            RuntimeMinutes: movie.runtimeMinutes || movie.RuntimeMinutes,
            AverageRating: movie.averageRating || movie.AverageRating,
            Language: movie.language || movie.Language,
            Genres: movie.genres ? (typeof movie.genres === 'string' ? movie.genres : movie.genres.join(',')) : (movie.Genres || ''),
            IsAdult: movie.isAdult !== undefined ? movie.isAdult : (movie.IsAdult !== undefined ? movie.IsAdult : false),
            NumVotes: movie.numVotes || movie.NumVotes,
            Budget: movie.budget || movie.Budget || "N/A", // Add budget
            GrossWorldwide: movie.grossWorldwide || movie.GrossWorldwide || "N/A", // Add box office
            RentStart: movie.rentStart || movie.RentStart,
            RentEnd: movie.rentEnd || movie.RentEnd,
            TotalPrice: movie.totalPrice || movie.TotalPrice,
            RentalCount: movie.rentalCount || movie.RentalCount || 0,
            RentalId: movie.rentalId || movie.RentalId,
        };

        renderMovie(normalizedMovie, btnType);
    });
}

// Pagination controls
function renderPagination(totalCount, currentPage, pageSize) {
    const totalPages = Math.ceil(totalCount / pageSize);
    const $pagination = $("#pagination");
    $pagination.empty();

    if (totalPages <= 1) return;

    for (let i = 1; i <= totalPages; i++) {
        const $btn = $(`<button class="pagination-btn">${i}</button>`);
        if (i === currentPage) $btn.addClass("active");
        $btn.click(() => getAllMoviesListFromServer(i, pageSize));
        $pagination.append($btn);
    }
}

function searchMoviesByTitle(movieTitle) {
    const normalizedTitle = movieTitle.trim();

    if (normalizedTitle === "") {
        console.log("Search input is empty. Reloading all movies...");
        getAllMoviesListFromServer(); // Reload all movies if the search input is empty
        return;
    }

    console.log(`Searching for movies with title: ${normalizedTitle}`);

    // Call the backend API to search for movies by title
    ajaxCall(
        "GET",
        `${getUrl}/search?title=${encodeURIComponent(normalizedTitle)}`, // Backend endpoint
        "",
        (filteredMovies) => {
            console.log("Filtered Movies from Server:", filteredMovies);

            if (filteredMovies.length === 0) {
                $("#loadedMovies").html("<p>No matches found for your search.</p>");
            } else {
                renderMyList(filteredMovies); // Render the filtered list of movies
            }
        },
        (err) => {
            console.error(`Error searching movies by title: ${err}`);
            alert("An error occurred while searching for movies. Please try again.");
        }
    );
}

function setupSearchFunctionality() {
    // Trigger search on button click
    $(`#titleSearchBTN`).click(() => {
        const movieTitle = $("input[name='movieSearch']").val();
        console.log(`Searching for movies with title: ${movieTitle}`);

        if (movieTitle.trim() === "") {
            // If the search input is empty, reload all movies
            getAllMoviesListFromServer();
        } else {
            searchMoviesByTitle(movieTitle);
        }
    });

    // Automatically reload movies when the search box is cleared
    $("input[name='movieSearch']").on("input", function () {
        const movieTitle = $(this).val();
        if (movieTitle.trim() === "") {
            console.log("Search box is empty. Reloading all movies...");
            getAllMoviesListFromServer(); // Reload all movies
        }
    });

    // Prevent form submission on Enter in the search box
    $("input[name='movieSearch']").closest("form").on("submit", function (e) {
        e.preventDefault();
    });

    // Search by date range
    $("#searchDateBTN").click(() => {
        const startDate = $("#startDate").val(); // Get the start date
        const endDate = $("#endDate").val(); // Get the end date

        console.log(`Start Date: ${startDate}, End Date: ${endDate}`); // Log the user input

        // Validate the input
        if (!startDate || !endDate) {
            alert("Please select both a start date and an end date.");
            return;
        }

        if (new Date(startDate) > new Date(endDate)) {
            alert("The start date cannot be after the end date.");
            return;
        }

        // Send a GET request to the server with the date range
        console.log(`Sending request to: ${getUrl}/searchByRouting/startDate/${startDate}/endDate/${endDate}`);

        ajaxCall(
            "GET",
            `${getUrl}/searchByDate?startDate=${startDate}&endDate=${endDate}`,
            "",
            (filteredMovies) => {
                console.log("Filtered Movies from Server:", filteredMovies); // Log the server response

                if (filteredMovies.length === 0) {
                    alert("No movies found for the selected date range.");
                } else {
                    renderMyList(filteredMovies); // Render the filtered list of movies
                }
            },
            (err) => {
                console.error(`Error filtering movies by date range: ${err}`);
                alert("An error occurred while filtering movies. Please try again.");
            }
        );
    });

    // Reset date inputs and reload all movies
    $("#resetDateBTN").click(() => {
        $("#startDate").val(""); // Clear the start date
        $("#endDate").val(""); // Clear the end date
        console.log("Date inputs cleared. Reloading all movies...");
        getAllMoviesListFromServer(); // Reload all movies
    });
}

function loadMyRentedMovies() {
    const userId = localStorage.getItem('loggedInId');
    if (!userId) {
        alert("You must be logged in to see your rented movies.");
        window.location.href = "login.html";
        return;
    }

    ajaxCall(
        "GET",
        `${backendBaseUrl}/Movie/rented/${userId}`,
        "",
        (moviesFromServer) => {
            if (moviesFromServer && moviesFromServer.length > 0) {
                renderMyList(moviesFromServer, "deleteFromList"); // Call delete from list
            } else {
                $("#loadedMovies").html("<p>You have no rented movies.</p>");
            }
        },
        (err) => {
            console.error("Error fetching rented movies", err);
            $("#loadedMovies").html("<p>Could not load your rented movies.</p>");
        }
    );
}

function deleteFromServer(movieId, movieDiv) {
    const userId = localStorage.getItem('loggedInId');
    // Only allow delete if on MyMovies.html (rented movies)
    if (window.location.pathname.endsWith('/pages/MyMovies.html')) {
        ajaxCall(
            "DELETE",
            `${backendBaseUrl}/Movie/rent/${userId}/${movieId}`,
            "",
            () => {
                // On success, remove the movie from the UI
                movieDiv.fadeOut(300, function () {
                    $(this).remove();
                    if ($("#loadedMovies .Movie").length === 0) {
                        $("#loadedMovies").append("<p>No movies in your collection yet.</p>");
                    }
                });
            },
            (err) => {
                console.error(`Error deleting rented movie: ${err}`);
                alert("Failed to remove rented movie. Please try again.");
            }
        );
    } else {
        alert("You can only remove movies from your personal collection (My Movies page).");
    }
}

/**
 * Displays a welcome message for the logged-in user on specific pages (index.html or MyMovies.html).
 * Checks if a user is logged in by retrieving their info from localStorage.
 */
function showGreeting() {
    const loggedInUser = localStorage.getItem('loggedInUser');
    const loggedInName = localStorage.getItem('loggedInName');
    const path = window.location.pathname;
    if (
        loggedInUser &&
        (path.endsWith('/pages/index.html') || path.endsWith('/pages/MyMovies.html') || path === '/')
    ) {
        const greetingName = loggedInName || loggedInUser;
        const greeting = `<h2 class="greeting">Welcome, ${greetingName}!</h2>`;
        // Insert greeting after the navbar
        $('#navbar').after(greeting);
    }
}

//------------------------
//Modal for renting
//------------------------
function calculateRentPrice(start, end, pricePerDay) {
    const msPerDay = 24 * 60 * 60 * 1000;
    const days = Math.max(1, Math.ceil((end - start) / msPerDay));
    return days * pricePerDay;
}
function openRentModal(movie) {
    // Frontend validation to prevent renting unreleased movies
    const releaseDate = new Date(movie.ReleaseDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (releaseDate > today) {
        alert("This movie has not been released yet and cannot be rented.");
        return; // Prevent modal from opening
    }

    $("#rentMovieTitle").text(movie.PrimaryTitle);

    // Set start date to today in yyyy-mm-dd format
    const yyyy = today.getFullYear();
    const mm = String(today.getMonth() + 1).padStart(2, '0');
    const dd = String(today.getDate()).padStart(2, '0');
    const todayStr = `${yyyy}-${mm}-${dd}`;
    $("#rentStart").val(todayStr);
    $("#rentEnd").val(""); // Reset end date
    $("#rentTotalPrice").text("");
    $("#rentModal").show();

    // Fetch price from server
    $.get(`${backendBaseUrl}/Movie/${movie.Id}`, function (movieDetails) {
        console.log(movieDetails);
        $("#rentModal").data("moviePrice", movieDetails.priceToRent); // Use PriceToRent
    });

    $("#rentModal").data("movie", movie);
}
function closeRentModal() {
    $("#rentModal").hide();
}

function setupRentModalEvents() {
    // Close modal on X
    $("#closeRentModal").click(closeRentModal);

    // Calculate price dynamically
    $("#rentStart, #rentEnd").on("change", function () {
        const start = new Date($("#rentStart").val());
        const end = new Date($("#rentEnd").val());
        const pricePerDay = $("#rentModal").data("moviePrice");

        console.log("Start date:", start);
        console.log("End date:", end);
        console.log("Price per day from modal data:", pricePerDay);

        if (pricePerDay === undefined) {
            $("#rentTotalPrice").text("Error: Could not fetch movie price.");
            console.error("Could not fetch movie price. Data on rentModal:", $("#rentModal").data());
            return;
        }
        if ($("#rentStart").val() && $("#rentEnd").val() && end >= start) {
            const price = calculateRentPrice(start, end, pricePerDay);
            console.log("Calculated price:", price);
            $("#rentTotalPrice").text("Total Price: $" + price);
        } else {
            $("#rentTotalPrice").text("");
        }
    });

    // Handle rent form submit
    $("#rentForm").on("submit", function (e) {
        e.preventDefault();
        const movie = $("#rentModal").data("movie");
        const userId = localStorage.getItem('loggedInId');
        const rentStart = $("#rentStart").val();
        const rentEnd = $("#rentEnd").val();

        if (!userId) {
            alert("You must be logged in to rent a movie.");
            window.location.href = 'login.html';
            return;
        }
        if (!rentStart || !rentEnd || new Date(rentEnd) < new Date(rentStart)) {
            alert("Please select valid dates.");
            return;
        }

        const rentRequest = {
            userId: Number(userId),
            movieId: movie.Id,
            rentStart: rentStart,
            rentEnd: rentEnd
        };

        ajaxCall(
            "POST",
            `${backendBaseUrl}/Movie/rent`,
            JSON.stringify(rentRequest),
            () => {
                const movieDiv = $(`#${movie.Id}`);
                movieDiv.attr("data-success-message", "✓ Rented");
                movieDiv.addClass("success");
                setTimeout(() => movieDiv.removeClass("success"), 3600);
                closeRentModal();
                // Optionally update UI or reload movies
            },
            (err) => {
                const movieDiv = $(`#${movie.Id}`);
                let failMsg = "✗ Failed to rent movie";

                if (err && err.responseText) {
                    console.log("Rent error details:", err.responseText);

                    // Match exact SP error messages
                    if (err.responseText.includes("You already have this movie rented for overlapping dates")) {
                        failMsg = "✗ You already have this movie rented for overlapping dates";
                    }
                    else if (err.responseText.includes("User is deleted and cannot rent movies")) {
                        failMsg = "✗ Your account has been deleted";
                    }
                    else if (err.responseText.includes("Movie is deleted and cannot be rented")) {
                        failMsg = "✗ This movie is no longer available";
                    }
                }

                movieDiv.attr("data-fail-message", failMsg);
                movieDiv.addClass("fail");
                setTimeout(() => movieDiv.removeClass("fail"), 3600);
                closeRentModal();
            }
        );
    });
}

//------------------------------------------------
//Modal for transfering movie to other user
//------------------------------------------------
function openTransferModal(movie) {
    $("#transferMovieTitle").text(movie.PrimaryTitle);
    $("#recipientEmail").val("");
    $("#transferError").text("").addClass("hidden");
    $("#transferModal").show();
    $("#transferModal").data("movie", movie);
}

function closeTransferModal() {
    $("#transferModal").hide();
}

function setupTransferModalEvents() {
    $("#closeTransferModal").click(closeTransferModal);

    $("#transferForm").on("submit", function (e) {
        e.preventDefault();
        const movie = $("#transferModal").data("movie");
        const recipientEmail = $("#recipientEmail").val().trim();
        const rentalId = movie.rentalId;

        if (!recipientEmail) {
            $("#transferError").text("Please enter a recipient email.").removeClass("hidden");
            return;
        }

        const userId = localStorage.getItem('loggedInId');
        if (!userId) {
            alert("You must be logged in to transfer a movie.");
            return;
        }

        ajaxCall(
            "POST",
            `${backendBaseUrl}/Movie/transferRental`,
            JSON.stringify({
                rentalId: rentalId,
                toUserEmail: recipientEmail
            }),
            () => {
                const movieDiv = $(`#${rentalId}`);
                movieDiv.attr("data-success-message", `✓ Movie transferred to ${recipientEmail}`);
                movieDiv.addClass("success");
                setTimeout(() => {
                    movieDiv.removeClass("success");
                    loadMyRentedMovies(); // refresh after the animation
                }, 4600);
                closeTransferModal();
            },
            (err) => {
                console.log("Transfer error callback triggered:", err);
                const movieDiv = $(`#${rentalId}`);
                let failMsg = "✗ Transfer failed";

                if (err && err.responseText) {
                    console.log("Error details:", err.responseText);

                    // Match exact SP error messages
                    if (err.responseText.includes("Recipient already has this movie rented for overlapping dates")) {
                        failMsg = "✗ Recipient already has this movie for overlapping dates";
                    }
                    else if (err.responseText.includes("Cannot transfer: rental period has ended")) {
                        failMsg = "✗ Cannot transfer: rental period has ended";
                    }
                    else if (err.responseText.includes("Recipient user not found")) {
                        failMsg = "✗ Recipient user not found";
                    }
                    else if (err.responseText.includes("Rental not found for transfer")) {
                        failMsg = "✗ Rental record not found";
                    }
                }

                movieDiv.attr("data-fail-message", failMsg);
                movieDiv.addClass("fail");
                setTimeout(() => movieDiv.removeClass("fail"), 3600);
                closeTransferModal();
            }
        );
    });
}

//---------------------------------------------------- Login/Register/Edit Pages Methods -------------------------------------------------
function handleLogin(event) {
    event.preventDefault();

    const email = $('#email').val();
    const password = $('#password').val();

    // Remove any previous messages
    $('.login-message').remove();

    function onLoginSuccess(response) {
        console.log("Login AJAX Success Response:", response);
        if (response) {
            // Success: Redirect or show a message
            $('form#loginForm').after('<p class="login-message success-message">Login successful! Redirecting...</p>');
            setTimeout(function () {
                localStorage.setItem('loggedInId', response.id);
                localStorage.setItem('loggedInUser', response.email);
                localStorage.setItem('loggedInName', response.name);
                window.location.href = "index.html"; // Use relative path for simplicity and robustness
            }, 3500);
        } else {
            $('form#loginForm').after('<p class="login-message error-message">Invalid email or password. Please try again.</p>');
        }
    }

    function onLoginError(xhr, status, error) {
        console.log("Login AJAX Error:", xhr, status, error);
        $('form#loginForm').after('<p class="login-message error-message">Login failed. Please try again later.</p>');
    }

    ajaxCall(
        "POST",
        loginUrl,
        JSON.stringify({ email, password }),
        onLoginSuccess,
        onLoginError
    );
}

function handleRegister(event) {
    event.preventDefault();

    const name = $('#name').val().trim();
    const email = $('#email').val().trim();
    const password = $('#password').val();

    // Regex for name: only letters, at least 2 characters
    const nameRegex = /^[A-Za-z\s]{2,}$/;
    // Regex for password: at least 8 chars, one number, one uppercase
    const passwordRegex = /^(?=.*\d)(?=.*[A-Z]).{8,}$/;

    if (!nameRegex.test(name)) {
        $('#registerError').text('Name must contain only letters and be at least 2 characters long.').removeClass('hidden');
        $('#registerSuccess').addClass('hidden');
        return;
    }
    if (!email) {
        $('#registerError').text('Email is required.').removeClass('hidden');
        $('#registerSuccess').addClass('hidden');
        return;
    }
    if (!passwordRegex.test(password)) {
        $('#registerError').text('Password must be at least 8 characters, include one number and one uppercase letter.').removeClass('hidden');
        $('#registerSuccess').addClass('hidden');
        return;
    }

    // Callback for success
    function onSuccess(response) {
        if (response && response.message && response.message.includes("success")) {
            // Registration succeeded
            $('#registerForm')[0].reset();
            $('#registerSuccess').text('Registration successful! Redirecting to login...').removeClass('hidden');
            $('#registerError').addClass('hidden');
            setTimeout(function () {
                window.location.href = 'login.html';
            }, 3500);
        } else {
            $('#registerError').text('Registration failed. Please try again.').removeClass('hidden');
            $('#registerSuccess').addClass('hidden');
        }
    }

    function onError() {
        $('#registerError').text('Registration failed. Please try again.').removeClass('hidden');
        $('#registerSuccess').addClass('hidden');
    }

    // Use ajaxCall with callbacks
    ajaxCall(
        "POST",
        registerUrl,
        JSON.stringify({ name, email, password }),
        onSuccess, // if i did onSuccess() it's wrong because this call the function immediately.
        onError
    );
}

function initEditProfileForm() {
    // Pre-fill form with localStorage values
    $("#editName").val(localStorage.getItem('loggedInName') || "");
    $("#editEmail").val(localStorage.getItem('loggedInUser') || "");

    $("#editProfileForm").on("submit", function (e) {
        e.preventDefault();
        const id = localStorage.getItem('loggedInId');
        const name = $("#editName").val().trim();
        const email = $("#editEmail").val().trim();
        const password = $("#editPassword").val();

        if (!id) {
            $("#editProfileMsg")
                .removeClass("success-message")
                .addClass("error-message")
                .text("User ID missing. Please log in again.");
            return;
        }

        const updatedUser = { id: Number(id), name, email, password, active: true };

        function onSuccess() {
            $("#editProfileMsg")
                .removeClass("error-message")
                .addClass("success-message")
                .text("Profile updated successfully!");
            localStorage.setItem('loggedInUser', email);
            localStorage.setItem('loggedInName', name);
            setTimeout(() => { window.location.href = "index.html"; }, 2000);
        }

        function onError() {
            $("#editProfileMsg")
                .removeClass("success-message")
                .addClass("error-message")
                .text("Failed to update profile.");
        }

        ajaxCall(
            "PUT",
            `${userApiBaseUrl}/${id}`,
            JSON.stringify(updatedUser),
            onSuccess,
            onError
        );
    });
}

//-------------------------------------------------------- Add Movie Page Methods ---------------------------------------------------------
// Add Movie Form logic
function initAddMovieForm() {
    $("#addMovieForm").on("submit", function (e) {
        e.preventDefault();

        // Clear previous errors
        $(".error-message").text("").addClass("hidden");
        $("input, textarea").removeClass("invalid");

        // Valid is true by default
        let valid = true;

        function setError(id, msg) {
            $("#" + id + "Error").text(msg).removeClass("hidden");
            $("#" + id).addClass("invalid");
            valid = false;
        }

        // Get values
        const url = $("#url").val().trim();
        const primaryTitle = $("#primaryTitle").val().trim();
        const description = $("#description").val().trim();
        const primaryImage = $("#primaryImage").val().trim();
        const year = $("#year").val().trim();
        const releaseDate = $("#releaseDate").val().trim();
        const language = $("#language").val().trim();
        const budget = $("#budget").val().trim();
        const grossWorldwide = $("#grossWorldwide").val().trim();
        const genres = $("#genres").val().trim();
        const isAdult = $("#isAdult").is(":checked");
        const runtimeMinutes = $("#runtimeMinutes").val().trim();
        const averageRating = $("#averageRating").val().trim();
        const numVotes = $("#numVotes").val().trim();
        const priceToRent = $("#priceToRent").val().trim();

        // Validation
        if (url && !/^https?:\/\/.+\..+/.test(url)) setError("url", "Please enter a valid URL.");
        if (!primaryTitle) setError("primaryTitle", "Primary title is required.");
        if (!primaryImage) setError("primaryImage", "Primary image is required.");
        else if (!/^https?:\/\/.+\..+/.test(primaryImage)) setError("primaryImage", "Please enter a valid image URL.");
        if (!year) {
            setError("year", "Year is required.");
        } else if (!/^\d{4}$/.test(year)) {
            setError("year", "Enter a valid year (e.g., 2024).");
        }

        if (!releaseDate) {
            setError("releaseDate", "Release date is required.");
        } else {
            const releaseYear = new Date(releaseDate).getFullYear(); // Extract the year from the release date
            if (releaseYear !== Number(year)) {
                setError("releaseDate", "Release date year must match the year field.");
            }
        }
        if (!language) setError("language", "Language is required.");
        if (budget && (isNaN(budget) || Number(budget) < 100000)) setError("budget", "Budget must be at least 100,000.");
        if (grossWorldwide && isNaN(grossWorldwide)) setError("grossWorldwide", "Gross Worldwide must be a number.");
        if (!genres) {
            setError("genres", "Genres are required.");
        } else {
            // Validate genres are comma-separated and contain valid words
            const genresArray = genres.split(",").map(g => g.trim());
            if (genresArray.some(g => !/^[A-Za-z\s]+$/.test(g) || g === "")) {
                setError("genres", "Genres must be valid words separated by commas (e.g., Action, Comedy).");
            }
        }
        if (!runtimeMinutes) setError("runtimeMinutes", "Runtime is required.");
        else if (isNaN(runtimeMinutes) || Number(runtimeMinutes) <= 0) setError("runtimeMinutes", "Enter a valid runtime.");
        if (averageRating && (isNaN(averageRating) || (Number(averageRating) < 0 || Number(averageRating) > 10))) setError("averageRating", "Average rating must be a number between 0 and 10.");
        if (numVotes && (isNaN(numVotes) || Number(numVotes) < 0)) setError("numVotes", "Number of votes must be a positive integer.");
        if (!priceToRent || isNaN(priceToRent) || Number(priceToRent) < 10 || Number(priceToRent) > 30) setError("priceToRent", "Price to rent is required and must be between 10 and 30.");

        if (!valid) {
            $("#addMovieError").text("Please fix the errors in the form.").removeClass("hidden");
            return;
        }

        // Prepare movie object (id is auto-generated on server)
        const movie = {
            url,
            primaryTitle,
            description,
            primaryImage,
            year: Number(year),
            releaseDate,
            language,
            budget: budget ? Number(budget) : 0,
            grossWorldwide: grossWorldwide ? Number(grossWorldwide) : 0,
            genres,
            isAdult,
            runtimeMinutes: Number(runtimeMinutes),
            averageRating: averageRating ? Number(averageRating) : 0,
            numVotes: numVotes ? Number(numVotes) : 0,
            priceToRent: Number(priceToRent)
        };

        console.log("Movie object being sent to the server:", movie);

        ajaxCall(
            "POST",
            postUrl,
            JSON.stringify(movie),
            function () {
                alert("Movie added successfully!");
                $("#addMovieForm")[0].reset();
            },
            function () {
                alert("Failed to add movie. Please try again.");
            }
        );
    });
}

//-----------------------------------------------------NavBar and Autrizion functionlty----------------------------------------------------
function loadNavbar() {
    // Dynamically load the navbar
    $("body").prepend('<div id="navbar"></div>'); // Placeholder for the navbar
    $("#navbar").load("navbar.html?v=" + new Date().getTime(), function () { // forces the browser to fetch the latest version of the file
        // Once the navbar is loaded, update it based on the user's login state
        updateNavbar();

        // Attach the sign-out logic to the Sign Out link
        $('#signOutLink').click(function (event) {
            event.preventDefault(); // Prevent the default link behavior
            handleSignOut();
        });
    });
}

function updateNavbar() {
    const loggedInUser = localStorage.getItem('loggedInUser');

    if (loggedInUser) {
        // User is logged in
        $('#loginLinks').hide(); // Hide Login/Register links
        $('#logoutLink').show(); // Show Sign Out button
    } else {
        // User is not logged in
        $('#loginLinks').show(); // Show Login/Register links
        $('#logoutLink').hide(); // Hide Sign Out button
    }
}

function handleSignOut() {
    localStorage.removeItem('loggedInId');
    localStorage.removeItem('loggedInUser');
    localStorage.removeItem('loggedInName');
    window.location.href = 'login.html';
}

function checkAuthorization() {
    const loggedInUser = localStorage.getItem('loggedInUser'); // Check if the user is logged in
    const path = window.location.pathname; // Get the current page path

    // Define public pages
    const publicPages = ['/pages/index.html', '/pages/login.html', '/pages/register.html', '/'];

    // If the user is not logged in and the current page is not public, redirect to login
    if (!loggedInUser && !publicPages.some(p => path.endsWith(p))) {
        window.location.href = 'login.html'; // Redirect to the login page
    }
}

//-----------------------------------------------------------------Admin functionlty-------------------------------------------------------
function initAdminPage() {
    fetchUsers(); // Load users initially

    // Setup event listeners for admin page elements
    const $searchInput = $('#searchInput');
    if ($searchInput.length) {
        $searchInput.on('keyup', searchUsers);
    }

    // Setup refresh button (give it an ID in your HTML for best practice)
    const $refreshButton = $('#refreshUsersBtn');
    if ($refreshButton.length) {
        $refreshButton.on('click', fetchUsers);
    }
}

function fetchUsers() {
    ajaxCall(
        "GET",
        `${backendBaseUrl}/User`,
        "",
        function (users) {
            // Filter out deleted users if deletedAt is present
            const filteredUsers = users.filter(user => !user.deletedAt);
            displayUsers(filteredUsers);
        },
        function (err) {
            console.error('Error fetching users:', err);
            alert('Failed to fetch users. Please try again.');
        }
    );
}

function displayUsers(users) {
    const $tableBody = $('#userTableBody');
    if (!$tableBody.length) return;
    $tableBody.empty();

    // Filter out deleted users(Only deleted ones)
    const filteredUsers = users.filter(user => !user.deletedAt);

    filteredUsers.forEach((user, index) => {
        const $row = $('<tr></tr>');
        $row.append(`<td>${index + 1}</td>`);
        $row.append(`<td>${user.name}</td>`);
        $row.append(`<td>${user.email}</td>`);
        $row.append(`<td>${user.isAdmin ? 'Admin' : 'User'}</td>`);
        $row.append(`<td>${user.active ? 'Active' : 'Inactive'}</td>`);

        const $actionsCell = $('<td></td>');
        const $switchLabel = $('<label class="switch"></label>');
        const $checkbox = $('<input type="checkbox">').prop('checked', user.active);
        $checkbox.on('change', () => toggleUserStatus(user.id, $checkbox.is(':checked')));

        const $slider = $('<span class="slider round"></span>');
        $switchLabel.append($checkbox).append($slider);
        $actionsCell.append($switchLabel);
        $row.append($actionsCell);
        $tableBody.append($row);
    });
}

function toggleUserStatus(userId, isActive) {
    ajaxCall(
        "PUT",
        `${userApiBaseUrl}/${userId}/active`,
        JSON.stringify(isActive),
        fetchUsers, // Refresh the list to show the updated status
        (err) => {
            console.error('Error updating user status:', err);
            alert('Failed to update user status. Please try again.');
        }
    );
}

function searchUsers() {
    const $input = $('#searchInput');
    if (!$input.length) return;
    const filter = $input.val().toLowerCase();
    const $tableBody = $('#userTableBody');
    if (!$tableBody.length) return;

    $tableBody.find('tr').each(function () {
        const $row = $(this);
        const nameText = $row.find('td:nth-child(2)').text().toLowerCase();
        const emailText = $row.find('td:nth-child(3)').text().toLowerCase();
        if (nameText.includes(filter) || emailText.includes(filter)) {
            $row.show();
        } else {
            $row.hide();
        }
    });
}