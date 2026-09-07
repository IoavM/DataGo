import { Routes } from '@angular/router';
import { ClientesListComponent } from './pages/clientes-list/clientes-list';
import { ClienteNuevoComponent } from './pages/cliente-nuevo/cliente-nuevo';

export const routes: Routes = [
  { path: '', redirectTo: 'clientes', pathMatch: 'full' },
  { path: 'clientes', component: ClientesListComponent },
  { path: 'clientes/nuevo', component: ClienteNuevoComponent },
  { path: '**', redirectTo: 'clientes' }
];
