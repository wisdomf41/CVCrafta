import { BrowserRouter } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "react-hot-toast";

const queryClient = new QueryClient();

function AppProviders({ children }) {
    return (
        <QueryClientProvider client={queryClient}>
            <BrowserRouter>
                {children}
                <Toaster position="top-right" />
            </BrowserRouter>
        </QueryClientProvider>
    );
}

export default AppProviders;