import AppProviders from "./app/AppProviders";
import AppRoutes from "./app/AppRoutes";
import AppNavbar from "./shared/components/AppNavbar";
import AppFooter from "./shared/components/AppFooter";

function App() {
    return (
        <AppProviders>
            <div className="flex min-h-screen flex-col">
                <AppNavbar />

                <main className="flex-1">
                    <AppRoutes />
                </main>

                <AppFooter />
            </div>
        </AppProviders>
    );
}

export default App;