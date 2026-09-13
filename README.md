# Kanishk Jewellers — ASP.NET Core

Luxury jewellery website with a responsive home page, admin login, jewellery catalogue, image uploads, search and category filters.

## Admin login
- **Shiva** — `2000`
- **Tanvi** — `1186`

The admin names are not displayed in the public navigation. These credentials are demo credentials; use secure authentication/password hashing for production.

## Features
- Luxury cream/gold home page matching the supplied reference design
- Responsive layout for desktop and mobile
- Jewellery categories: Necklace, Ring, Earrings, Bracelet, Bangle, Chain, Pendant, Nose Ring, Anklet, Mangalsutra, Other
- Add, edit and delete jewellery (admin only)
- Image upload: JPG, JPEG, PNG, WEBP
- Image preview/lightbox
- Search and category filter
- SQLite database
- No jewellery price field

## Run locally

Open the folder containing `KanishkJewellers.csproj` in VS Code, then:

```bash
dotnet restore
dotnet run
```

Open the localhost address printed by the terminal.

## GitHub upload

Upload the **contents of this project folder** to the root of your GitHub repository. Do not upload `bin/`, `obj/`, local `data/`, or customer photos from `wwwroot/uploads/`.

`wwwroot/images/` contains the permanent design images used by the home page.

## Render deployment + permanent photo storage

For a public deployment on Render, configure:

- Build command: `dotnet publish -c Release -o out`
- Start command: `dotnet out/KanishkJewellers.dll`
- Environment variable: `DATA_DIR=/var/data`
- Persistent Disk mounted at: `/var/data`

The SQLite database and uploaded jewellery photos are stored under `DATA_DIR`. Without a persistent disk, uploaded photos/database data can be lost when the service is redeployed or restarted.

## Project structure

- `Controllers/` — Home, Account and Jewellery logic
- `Models/` — Jewellery model
- `Data/` — SQLite Entity Framework context
- `Views/` — Razor pages
- `wwwroot/css/site.css` — luxury responsive design
- `wwwroot/images/` — home page images
- `wwwroot/uploads/` — runtime customer uploads (kept empty in Git)
