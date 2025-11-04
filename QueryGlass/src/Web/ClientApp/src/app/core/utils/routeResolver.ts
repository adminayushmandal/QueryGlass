import { ResolveFn } from "@angular/router";
import { User, UserService } from "../services/user.service";
import { inject, resource, ResourceStatus } from "@angular/core";
import { catchError, of } from "rxjs";
import { environment } from "src/environments/environment";

export const userResolver: ResolveFn<User | null> = () => {
    const userService = inject(UserService)
    const currentUser = userService.currentUser()

    if (currentUser) return of(currentUser)

    return userService.fetchUser()
        .pipe(
            catchError(err => {
                if (!environment.production) {
                    console.log("Failed to fetch the user", err);
                }
                return of(null)
            })
        )
}